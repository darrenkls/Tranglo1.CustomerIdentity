using IdentityServer4.Events;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Domain.Entities.Specifications.CustomerUserBusinessProfiles;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class ValidateUserPasswordCommand : IRequest<SignInResultDTO>
    {
        public string Username { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string RecaptchaToken { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public class ValidateUserPasswordCommandHandler : IRequestHandler<ValidateUserPasswordCommand, SignInResultDTO>
        {
            private readonly IBusinessProfileRepository _businessProfileRepository;
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ApplicationUserDbContext _appUserDbContext;
            private readonly SignInManager<ApplicationUser> _SignInManager;
            private readonly ICapchaValidator _CapchaValidator;
            private readonly TrangloUserManager _userManager;
            private readonly IEventService _events;
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly ILogger<ValidateUserPasswordCommandHandler> _logger;
            private readonly IConfiguration _Configuration;
            public ValidateUserPasswordCommandHandler(
                ApplicationUserDbContext appUserDbContext,
                IApplicationUserRepository applicationUserRepository,
                IBusinessProfileRepository businessProfileRepository,
                SignInManager<ApplicationUser> signInManager,
                ICapchaValidator capchaValidator,
                TrangloUserManager userManager, IEventService events,
                IHttpClientFactory httpClientFactory,
                ILogger<ValidateUserPasswordCommandHandler> logger,
                IConfiguration configuration)
            {
                _Configuration = configuration;
                _appUserDbContext = appUserDbContext;
                _businessProfileRepository = businessProfileRepository;
                _applicationUserRepository = applicationUserRepository;
                _SignInManager = signInManager;
                _CapchaValidator = capchaValidator;
                _userManager = userManager;
                _events = events;
                _httpClientFactory = httpClientFactory;
                _logger = logger;
            }

            public async Task<SignInResultDTO> Handle(ValidateUserPasswordCommand request, CancellationToken cancellationToken)
            {
                var capchaResult = await _CapchaValidator.ValidateAsync(request.RecaptchaToken);
                ApplicationUser loginedUser = await _userManager.FindByIdAsync(request.Username);
                string password = request.Password;
                bool isTpnResetPassword = false;
                string resetPasswordToken = "";
                MFA mfa = await _applicationUserRepository.GetMFAAsync(loginedUser);

                if (loginedUser is CustomerUser)
                {
                    var customerUser = (CustomerUser)loginedUser;
                    if (customerUser.IsTPNUser)
                    {
                        using (var client = this._httpClientFactory.CreateClient())
                        {
                            try
                            {
                                string soapEnvelope = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/""><soapenv:Header/>
                                    <soapenv:Body><tem:oneWayEncrypt><!--Optional:--><tem:plainText>" + request.Password + @"</tem:plainText>
                                    <!--Optional:--><tem:crytoKey>" + _Configuration["SoapCMSCryptoKey"] + @"</tem:crytoKey></tem:oneWayEncrypt>
                                    </soapenv:Body></soapenv:Envelope>";
                                var req = new HttpRequestMessage
                                {
                                    Method = HttpMethod.Post,
                                    Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml"),
                                    RequestUri = new Uri(_Configuration["soapCMSApi"] + "/CoreInteractionService.svc?wsdl"),
                                };
                                req.Headers.Add("SOAPAction", "http://tempuri.org/ICoreInteractionService/oneWayEncrypt");
                                var respond = await client.SendAsync(req);
                                if (respond.StatusCode == HttpStatusCode.OK)
                                {
                                    var respondContent = await respond.Content.ReadAsStringAsync();
                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.LoadXml(respondContent);
                                    var nodes = xmlDoc.GetElementsByTagName("oneWayEncryptResult");
                                    var hashedPassword = nodes.Item(0).InnerText;
                                    if (hashedPassword == customerUser.PasswordHash)
                                    {
                                        resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(customerUser);
                                        resetPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));
                                        isTpnResetPassword = true;
                                    }
                                }
                                else
                                {
                                    _logger.LogError("ValidateUserPasswordCommand - SOAP API error");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("ValidateUserPasswordCommand", ex.Message);
                            }
                        }
                    }  
                }

                SignInResultDTO resultDTO = new SignInResultDTO();
                resultDTO.SignInResult = null;
                resultDTO.IsResetPassword = isTpnResetPassword;
                resultDTO.ResetPasswordToken = resetPasswordToken;
                if(mfa != null)
                {
                    resultDTO.AuthenticationType = mfa.AuthenticationType;
                }
                
                if (!isTpnResetPassword)
                {
                    bool lockout = loginedUser is CustomerUser customer;
                    
                    SignInResult result = await _SignInManager.PasswordSignInAsync(request.Username, password, request.RememberLogin, lockoutOnFailure: lockout);
                    resultDTO.SignInResult = result;

                    
                    if (result.Succeeded && capchaResult.IsSuccess)
                    {
                        ApplicationUser user = await _userManager.FindByIdAsync(request.Username);

                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.FullName.Value, user.Id.ToString(), user.LoginId, clientId: null));
                        await _applicationUserRepository.UpdateApplicationUser(loginedUser, cancellationToken);

                        return resultDTO;
                    }

                    if (result.IsLockedOut && loginedUser is CustomerUser)
                    {
                        await _events.RaiseAsync(new UserLoginFailureEvent(request.Username, "user account has been lock", clientId: null));

                        // Update user account status
                        ApplicationUser user = await _userManager.FindByIdAsync(request.Username);
                        user.SetAccountStatus(AccountStatus.Blocked);
                        var updateResult = _appUserDbContext.ApplicationUsers.Update(user);
                        await _appUserDbContext.SaveChangesAsync(cancellationToken);

                        // Update account status for CustomerUserBusinessProfiles
                        var byUserID = new CustomerUserBusinessProfileByUserID(user.Id);
                        var customerUserBusinessProfileList = await _businessProfileRepository.GetCustomerUserBusinessProfilesAsync(byUserID);

						var userBlockStatus = customerUserBusinessProfileList
	                                        .Select(p => p.CompanyUserBlockStatus)
	                                        .FirstOrDefault(x => x.Id == CompanyUserBlockStatus.Block.Id);
                        if (userBlockStatus == null)
							userBlockStatus = await _applicationUserRepository.GetCompanyUserBlockStatusAsync(CompanyUserBlockStatus.Block);

						foreach (var profile in customerUserBusinessProfileList)
						{
                            profile.SetCompanyUserBlockStatus(userBlockStatus);
						}
						await _businessProfileRepository.UpdateCustomerUserBusinessProfileRangeAsync(customerUserBusinessProfileList.ToList(), cancellationToken);

                        return resultDTO;
                    }

                    await _events.RaiseAsync(new UserLoginFailureEvent(request.Username, "invalid credentials", clientId: null));
                    await _applicationUserRepository.UpdateApplicationUser(loginedUser, cancellationToken);
                    return resultDTO;
                }
                return resultDTO;

            }
        }
    }
}
