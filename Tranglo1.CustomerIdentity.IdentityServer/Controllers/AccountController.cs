// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using AutoMapper.QueryableExtensions;
using CorrelationId.Abstractions;
using CSharpFunctionalExtensions;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Command;
using Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Meta;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.MFA;
using Tranglo1.CustomerIdentity.IdentityServer.Models;
using Tranglo1.CustomerIdentity.IdentityServer.Models.MFA;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Services;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Tranglo1.UserAccessControl;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers
{
    //[SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICountrySettingRepository _repository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IMapper _mapper;
        private readonly ApplicationUserDbContext _IdentityServercontext;
        private readonly TrangloUserManager _userManager;
        private readonly ISignUpCodeRepository _signUpCodeRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IHostEnvironment _env;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private readonly IAuditLogService _auditLogService;

        public IMediator Mediator { get; }
        private readonly IConfiguration _config;
        private readonly ILogger<AccountController> _logger;

        private readonly UrlEncoder _urlEncoder;

        public AccountController(
            //UserManager<ApplicationUser> userManager,
            ICountrySettingRepository repository,
            TrangloUserManager userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IMapper mapper,
            AccessControlManager accessControlManager,
            ApplicationUserDbContext IdentityServercontext,
            IMediator mediator, IConfiguration config,
            ILogger<AccountController> logger,
            IBusinessProfileRepository businessProfileRepository,
            ISignUpCodeRepository signUpCodeRepository,
            IPartnerRepository partnerRepository,
            UrlEncoder urlEncoder,
            IApplicationUserRepository applicationUserRepository,
            ICorrelationContextAccessor correlationContextAccessor,
            IAuditLogService auditLogService,
            IHostEnvironment env)
        {
            _repository = repository;
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _events = events;
            _mapper = mapper;
            _IdentityServercontext = IdentityServercontext;
            Mediator = mediator;
            _config = config;
            _logger = logger;
            _businessProfileRepository = businessProfileRepository;
            _signUpCodeRepository = signUpCodeRepository;
            _partnerRepository = partnerRepository;
            _urlEncoder = urlEncoder;
            _applicationUserRepository = applicationUserRepository;
            _correlationContextAccessor = correlationContextAccessor;
            _auditLogService = auditLogService;
            _env = env;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login(string returnUrl)
        {
            var viewModel = new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.Username);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
                    return View(await BuildLoginViewModelAsync(model));
                }
                if (user.AccountStatus == AccountStatus.Inactive)
                {
                    ModelState.AddModelError(string.Empty, "Account is inactive.");
                    return View(await BuildLoginViewModelAsync(model));
                }
                if (user.LockoutEnd != null && DateTimeOffset.Compare(user.LockoutEnd.Value, new DateTimeOffset(DateTime.UtcNow)) > 0)
                {
                    ModelState.AddModelError(string.Empty, "Account locked.");
                    return View(await BuildLoginViewModelAsync(model));
                }

                ValidateUserPasswordCommand command = _mapper.Map<ValidateUserPasswordCommand>(model);
                command.AccountStatus = user.AccountStatus;
                var result = await Mediator.Send(command);
                if (result.SignInResult != null)
                {
                    var mfa = await _applicationUserRepository.GetMFAAsync(user);
                    if (result.SignInResult.Succeeded)
                    {
                        TempData["returnUrl"] = model.ReturnUrl;

                        if (!result.SignInResult.RequiresTwoFactor)
                        {
                            if (!(user is CustomerUser))
                            {
                                return RedirectToAction(actionName: "Enable2FA", controllerName: "Account", new { username = model.Username });
                            }
                            else
                            {
                                if (mfa == null)
                                {
                                    // setup email authentication for user
                                    await _applicationUserRepository.SetMFAAsync(user, AuthenticationType.Authenticator_Application, null, null);
                                    await _applicationUserRepository.CustomSetTwoFactorEnabledAsync(user, true);
                                }

                                return RedirectToAction(actionName: "Enable2FA", controllerName: "Account", new { username = model.Username });
                            }
                        }
                    }
                    else if (result.SignInResult.RequiresTwoFactor)
                    {
                        if (user is CustomerUser && result.AuthenticationType.Id == AuthenticationType.Email.Id)
                        {
                            await _applicationUserRepository.RemoveMFAAsync(user);
                            await _applicationUserRepository.CustomSetTwoFactorDisabledAsync(user, false);
                            await _applicationUserRepository.SetMFAAsync(user, AuthenticationType.Authenticator_Application, null, null);
                            await _applicationUserRepository.CustomSetTwoFactorEnabledAsync(user, true);

                            return RedirectToAction(actionName: "Enable2FA", controllerName: "Account", new { username = model.Username });
                        }
                        if (user is CustomerUser &&
                            result.AuthenticationType.Id == AuthenticationType.Authenticator_Application.Id && mfa.RecoveryCode == null)
                        {
                            await _applicationUserRepository.RemoveMFAAsync(user);
                            await _applicationUserRepository.CustomSetTwoFactorDisabledAsync(user, false);
                            await _applicationUserRepository.SetMFAAsync(user, AuthenticationType.Authenticator_Application, null, null);
                            await _applicationUserRepository.CustomSetTwoFactorEnabledAsync(user, true);

                            return RedirectToAction(actionName: "Enable2FA", controllerName: "Account", new { username = model.Username });
                        }
                        else
                        {
                            TempData["returnUrl"] = model.ReturnUrl;

                            // Reset AccessFailedCount if login passed TBT-1806
                            IdentityResult ResetAccessFailedCountResponse = await _userManager.ResetAccessFailedCountAsync(user);
                            if (!ResetAccessFailedCountResponse.Succeeded)
                            {
                                _logger.LogError($"[Login Controller] ResetAccessFailedCount. {ResetAccessFailedCountResponse.Errors}");
                            }

                            return RedirectToAction("LoginWith2fa");
                        }
                    }
                    else if (result.SignInResult.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Account locked.");
                        return View(await BuildLoginViewModelAsync(model));
                    }
                }
                else if (result.IsResetPassword)
                {
                    if (user is CustomerUser)
                    {
                        return Redirect(_config.GetValue<string>("IdentityServerUri") + "/account/createpassword?email=" + model.Username + "&token=" + result.ResetPasswordToken);
                    }
                }

                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            await _auditLogService.PersistAuditLogAsync(DateTime.UtcNow, "Logout",
                HttpContext?.Connection?.RemoteIpAddress, _correlationContextAccessor.CorrelationContext.CorrelationId);

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            //If no post logout redirect url is found, we just route browser to SSO login page.
            if (string.IsNullOrEmpty(vm.PostLogoutRedirectUri))
            {
                return RedirectToAction("Login", "Account");
            }

            return Redirect(vm.PostLogoutRedirectUri);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordEmail()
        {
            // Retrieve the email from TempData
            var email = TempData["ResetEmail"] as string;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            var viewModel = new ForgotPasswordEmailViewModel
            {
                Email = email
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var command = _mapper.Map<RequestResetPasswordCommand>(model);
            await Mediator.Send(command);

            // Store the email in TempData (server-side) instead of the URL
            TempData["ResetEmail"] = model.Email;

            // Redirect without passing the email in the route parameters
            return RedirectToAction("ForgotPasswordEmail", "Account");
        }

        [HttpGet]
        public IActionResult CreatePassword(string email = null, string token = null)
        {
            var vm = new CreatePasswordModel()
            {
                Email = email,
                Token = token
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePassword(CreatePasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token))
            {
                ModelState.AddModelError("Error", "Please ensure the reset password link is correct.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var command = _mapper.Map<VerifyCustomerUserResetPasswordCommand>(model);
                var result = await Mediator.Send(command);

                if (result.Succeeded)
                {
                    return RedirectToAction("SuccessResetPassword", "Account");
                }
                else if (result.Errors != null)
                {

                    ModelState.AddModelError("Error", result.Errors.First().Description);
                }
            }

            return View(model);
        }

        public IActionResult SuccessResetPassword()
        {
            return View();
        }

        /// <summary>
        /// Switcher
        /// </summary>
        /// <param name="cancellationToken">The object used to register user.</param>
        /// <returns>IdentityResult</returns>
        [HttpGet]
        //[Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        public async Task<IActionResult> Switcher(CancellationToken cancellationToken = default)
        {
            SwitcherViewModel model = new SwitcherViewModel();

            //Get Solutions Query
            var solutions = await _IdentityServercontext.Solutions.ProjectTo<SolutionListOutputDTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            model.Solutions = solutions.FindAll(a => a.SolutionCode == Solution.Connect.Id || a.SolutionCode == Solution.Business.Id);

            //Get user email
            var loginId = User.GetSubjectId();
            ApplicationUser user = await _userManager.FindByIdAsync(loginId);
            model.FullName = user.FullName.Value;

            return View(model);
        }

        /// <summary>
        /// Register user.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="signupcode"></param>
        /// <param name="command">The object used to register user.</param>
        /// <returns>IdentityResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CancellationToken cancellationToken = default, bool signupcode = false)
        {
            RegisterViewModel model = new RegisterViewModel()
            {
                HasSignUpCode = signupcode
            };

            await this.PopulateRegisterViewDropdown(model, cancellationToken);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterWithRegistryCode(CancellationToken cancellationToken = default)
        {
            RegisterWithRegistryCodeViewModel model = new RegisterWithRegistryCodeViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                var command = _mapper.Map<RegisterCustomerUserCommand>(model);
                command.ModelState = ModelState;

                try
                {
                    var identityResult = await Mediator.Send(command);

                    if (identityResult.Succeeded)
                    {
                        return RedirectToAction("RegistrationSuccess", "Account", new { email = model.Email });
                    }

                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError(string.Empty, ex.Message);
                    //Should show a friendly message to front end user if there is any internal error
                    _logger.LogError($"Error: {ex.StackTrace} and {ex.InnerException}");
                    return RedirectToAction("Error", "Home");

                }
            }

            await this.PopulateRegisterViewDropdown(model, cancellationToken);

            var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWithRegistryCode(RegisterWithRegistryCodeViewModel model, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                var command = _mapper.Map<RegisterCustomerUserWithCodeCommand>(model);
                command.ModelState = ModelState;
                try
                {
                    var identityResult = await Mediator.Send(command);

                    if (identityResult.Succeeded)
                    {
                        return RedirectToAction("RegistrationSuccess", "Account", new { email = model.Email });
                    }

                    //foreach (var error in identityResult.Errors)
                    //{
                    //    ModelState.AddModelError(string.Empty, error.Description);
                    //}
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError(string.Empty, ex.Message);
                    //Should show a friendly message to front end user if there is any internal error
                    return RedirectToAction("Error", "Home", new { errorId = ex.Message });

                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<int> GetSignUpCodeData(string signUpCode)
        {
            //GetSignUpCode from repo
            //From Sign Up Code get partner & business profile to know the solution & customer type
            //Return the sign up code data on solution and customer type
            int showPICName = 0;

            var signUpCodeObj = await _signUpCodeRepository.GetSignUpCodesAsync(signUpCode);

            if (signUpCodeObj is null)
            {
                return showPICName;
            }

            var partner = await _partnerRepository.GetPartnerRegistrationByCodeAsync(signUpCodeObj.PartnerCode);

            if (partner is null)
            {
                return showPICName;
            }
            var partnerSubscriptions = await _partnerRepository.GetPartnerSubscriptionListAsync(signUpCodeObj.PartnerCode);
            var existsBusinessSolution = partnerSubscriptions.Exists(x => x.Solution == Solution.Business);
            var existsConnectSolution = partnerSubscriptions.Exists(x => x.Solution == Solution.Connect);

            if (partner.CustomerTypeCode == CustomerType.Individual.Id && existsBusinessSolution)
            {
                showPICName = 2;
            }
            else if (existsBusinessSolution || existsConnectSolution)
            {
                showPICName = 1;
            }

            //0 = not valid
            //1 = show PIC Name
            //2 = hide PIC Name
            return showPICName;
        }

        public async Task PopulateRegisterViewDropdown(RegisterViewModel model, CancellationToken cancellationToken)
        {
            //Populate from Country
            //var countriesMeta = CountryMeta.GetNonSanctionCountriesAsync();
            var countriesMeta = await _repository.GetIsDisplayCountriesAsync();

            var config = new MapperConfiguration
                (cfg =>
                    cfg.CreateMap<CountryMeta, CountryListOutputDTO>()
                    .ForMember(o => o.Description, act => act.MapFrom(m => m.Name))
                );

            var mapper = new Mapper(config);
            model.Countries = mapper.Map<IReadOnlyList<CountryListOutputDTO>>(countriesMeta);

            //Get Solutions Query
            var solutions = await _IdentityServercontext.Solutions.ProjectTo<SolutionListOutputDTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            model.Solutions = solutions.FindAll(a => a.SolutionCode == Solution.Business.Id || a.SolutionCode == Solution.Connect.Id);

            ////Get User Types Query
            //var userTypes = await _IdentityServercontext.UserTypes.ProjectTo<UserTypeListOutputDTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            //model.UserTypes = userTypes.FindAll(a => a.UserTypeCode != Domain.Entities.UserType.Undefined.Id);

            //Get Customer Types Query
            var customerTypes = await _IdentityServercontext.CustomerTypes.ProjectTo<CustomerTypeListOutputDTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            model.CustomerTypes = customerTypes;

            //Get FindUs Query
            var leadsOrigin = Enumeration.GetAll<PartnerRegistrationLeadsOrigin>().OrderBy(x => x.Id)
                    .Select(x => new PartnerRegistrationLeadsOriginOutputDTO
                    {
                        PartnerRegistrationLeadsOriginCode = x.Id,
                        Description = x.Name
                    });
            model.PartnerRegistrationLeadsOrigin = leadsOrigin;
        }

        [HttpGet]
        public IActionResult RegistrationSuccess(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        /// <summary>
        /// Verify user email.
        /// </summary>
        /// <param name="command">The object used to verify user email.</param>
        /// <returns>RedirectResult</returns>
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromQuery] VerifyCustomerUserEmailModel model)
        {
            var command = _mapper.Map<VerifyCustomerUserEmailCommand>(model);
            try
            {
                IdentityResult identityResult = await Mediator.Send(command);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("EmailValidationSuccess", "Account");
                }
            }
            catch
            {
                return RedirectToAction("EmailValidationFailed", "Account");
            }
            return RedirectToAction("EmailValidationFailed", "Account");
        }

        /// <summary>
        /// resend user email verification.
        /// </summary>
        /// <returns>RedirectResult</returns>
        [HttpPost]
        public async Task<IActionResult> ResendEmailVerification(string email)
        {
            ResendEmailVerificationCommand command = new ResendEmailVerificationCommand();
            {
                command.Email = email;

            }
            ;
            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                return ValidationProblem(result.Error);
            }
            return Ok(result);
        }

        [HttpGet]
        public IActionResult EmailValidationSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EmailValidationFailed()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> InviteePasswordVerification(string email = null, string token = null)
        {
            CustomerUser user = await _userManager.FindByIdAsync(email) as CustomerUser;

            // verify token
            var tokenString = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            bool verifyToken = await _userManager.VerifyUserTokenAsync(user, "InvitationDataProtectorTokenProvider", "Invitation", tokenString);

            if (verifyToken)
            {
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var vm = new InviteePasswordVerificationViewModel()
                {
                    LoginId = email,
                    Name = user.FullName.Value,
                    ResetPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(passwordResetToken))
                };
                return View(vm);
            }
            else
            {
                ModelState.AddModelError("Error", "Token is invalid.");
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteePasswordVerification(InviteePasswordVerificationViewModel model)
        {
            var command = _mapper.Map<InviteePasswordVerificationCommand>(model);
            try
            {
                IdentityResult identityResult = await Mediator.Send(command);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("EmailValidationSuccess", "Account");
                }
            }
            catch
            {
                return RedirectToAction("EmailValidationFailed", "Account");
            }
            return RedirectToAction("EmailValidationFailed", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Enable2FA(string userName)
        {
            //var user = await _userManager.GetUserAsync(User);
            ApplicationUser user = await _userManager.FindByIdAsync(userName);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await LoadSharedKeyAndQrCodeUriAsync(user);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Enable2FA(EnableMultiFactorAuthenticationViewModel model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user);
            }

            var isValidToken = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.Code);

            if (!isValidToken)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");
                EnableMultiFactorAuthenticationViewModel viewModel = new EnableMultiFactorAuthenticationViewModel();
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return View(viewModel);
            }

            await _applicationUserRepository.CustomSetTwoFactorEnabledAsync(user, true);

            await _auditLogService.PersistAuditLogAsync(DateTime.UtcNow, "Activate 2FA",
                HttpContext?.Connection?.RemoteIpAddress, _correlationContextAccessor.CorrelationContext.CorrelationId);

            var validRecoveryCodeCount = await _userManager.CountRecoveryCodesAsync(user);

            if (validRecoveryCodeCount == 0)
            {
                var recoveryCode = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                TempData["RecoveryCodes"] = recoveryCode.ToArray();
            }
            return RedirectToAction("ShowRecoveryCodes");
        }

        [HttpGet]
        public async Task<IActionResult> ShowRecoveryCodes(ShowRecoveryCodesViewModel model)
        {
            if (model.RecoveryCodes == null && model.StatusMessage == null)
            {
                string[] recoveryCodes = TempData["RecoveryCodes"] as string[];

                model.RecoveryCodes = recoveryCodes;
                model.StatusMessage = TempData["StatusMessage"] as string ?? "";
            }

            if (model.RecoveryCodes == null || model.RecoveryCodes.Length == 0)
            {
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> LoginWith2fa()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var mfa = await _applicationUserRepository.GetMFAAsync(user);

            if (mfa.AuthenticationType == AuthenticationType.Email)
            {
                RequestEmailAuthenticationOTPCommand command = new RequestEmailAuthenticationOTPCommand
                {
                    LoginId = user.LoginId,
                };

                var result = await Mediator.Send(command);

                if (result.IsFailure)
                {
                    ModelState.AddModelError("Error", result.Error);
                    _logger.LogError($"[MFAEmail] {result.Error}");
                    return RedirectToAction("Login");
                }
                //return Ok(result.Value);
            }

            LoginWith2faViewModel model = new LoginWith2faViewModel();
            model.AuthenticationType = mfa.AuthenticationType.Id;

            return View(model);
        }

        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var returnUrl = TempData["returnUrl"]?.ToString();

            if (model.AuthenticationType == AuthenticationType.Authenticator_Application.Id)
            {
                if (_config.GetValue<bool>("ByPass2FA") && model.Code == "999999")
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return await LoginRedirect(returnUrl, user); // or whatever redirect logic you have

                    // fallback or error
                }

                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.Code, false, false);

                if (result.Succeeded)
                {
                    return await LoginRedirect(returnUrl, user);
                }

                model.Error = "Invalid OTP.";
                _logger.LogError($"[ValidateMFAOTP] Invalid OTP");
            }
            else if (model.AuthenticationType == AuthenticationType.Email.Id)
            {
                ValidateEmailAuthenticationOTPQuery query = new ValidateEmailAuthenticationOTPQuery
                {
                    MFAOTP = model.Code,
                    user = user
                };

                var result = await Mediator.Send(query);

                if (result.Value != false)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return await LoginRedirect(returnUrl, user);
                }

                model.Error = "Invalid OTP.";
                _logger.LogError($"[ValidateMFAOTP] Invalid OTP");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RedeemRecoveryCode()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RedeemRecoveryCode(RedeemRecoveryCodesViewModel model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await CustomTwoFactorRecoveryCodeSignInAsync(model.RecoveryCode);

            if (!result.Succeeded)
            {
                return View();
            }

            var returnUrl = TempData["returnUrl"]?.ToString();


            return await LoginRedirect(returnUrl, user);
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                //if (!local)
                //{
                //    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                //}

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            //var providers = schemes
            //    .Where(x => x.DisplayName != null)
            //    .Select(x => new ExternalProvider
            //    {
            //        DisplayName = x.DisplayName ?? x.Name,
            //        AuthenticationScheme = x.Name
            //    }).ToList();

            //var allowLocal = true;
            //if (context?.Client.ClientId != null)
            //{
            //    var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
            //    if (client != null)
            //    {
            //        allowLocal = client.EnableLocalLogin;

            //        if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
            //        {
            //            providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
            //        }
            //    }
            //}

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                //ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        private async Task<IActionResult> LoginRedirect(string returnUrl, ApplicationUser user)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else if (string.IsNullOrEmpty(returnUrl))
            {
                if (user is CustomerUser)
                {
                    //return Redirect(_config.GetValue<string>("ConnectPortalUri"));

                    // Replace redirect to portal with routing to switcher:
                    // 1) Create instance of SwitcherModel --> Create SwitcherInputModel & SwitcherViewModel
                    // 2) Return View with SwitcherViewModel
                    // 3) Create POST to handle routing between TB and TC portal (Param: SwitcherInputModel)

                    //TODO: Ensure that the email passed over is a valid one for the selected connect / business.
                    //1. ONLY if the email has both business and connect solution then we will bring them over to the switcher page
                    //If not then it should route directly to business or connect directly

                    //if( connect )
                    //return Redirect(_config.GetValue<string>("ConnectPortalUri"));
                    //else if( business )
                    //return Redirect(_config.GetValue<string>("BusinessUri"));

                    List<Solution> userSolutions = await _businessProfileRepository.GetSolutionsByUserAsync(user.Id);

                    if (userSolutions.Count() == 1)
                    {
                        if (userSolutions.First() == Solution.Connect)
                        {
                            return Redirect(_config.GetValue<string>("ConnectPortalUri"));
                        }
                        else if (userSolutions.First() == Solution.Business)
                        {
                            return Redirect(_config.GetValue<string>("BusinessPortalUri"));
                        }
                    }
                    else
                    {
                        return RedirectToAction(actionName: "Switcher", controllerName: "Account");
                    }

                }
                else
                {
                    return Redirect(_config.GetValue<string>("IntranetUri"));
                }
            }
            else
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
            return View();
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            ViewData["SharedKey"] = FormatKey(unformattedKey);

            ViewData["AuthenticatorUri"] = GenerateQrCodeUri(user.LoginId, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string loginId, string unformattedKey)
        {
            return string.Format(
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                _urlEncoder.Encode($"[{_env.EnvironmentName}] Tranglo1"),
                _urlEncoder.Encode(loginId),
                unformattedKey);
        }

        public async Task<SignInResult> CustomTwoFactorRecoveryCodeSignInAsync(string recoveryCode)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return SignInResult.Failed;
            }

            var mfa = await _applicationUserRepository.GetMFAAsync(user);

            if (mfa != null)
            {
                var storedRecoveryCode = mfa.RecoveryCode;

                if (storedRecoveryCode == null || !storedRecoveryCode.Contains(recoveryCode))
                {
                    return SignInResult.Failed;
                }
            }

            await _applicationUserRepository.CustomSetTwoFactorEnabledAsync(user, false);
            await _applicationUserRepository.RemoveMFAAsync(user);

            await _signInManager.SignInAsync(user, isPersistent: false);
            return SignInResult.Success;
        }

        [HttpGet]
        public IActionResult UnsubscribeKYCReminder(UnsubscribeKYCReminderModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> KYCReminderUnsubscribe(UnsubscribeKYCReminderModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid request model.");
            }

            var query = new KYCUnSubscriptionCommand
            {
                PartnerSubscriptionCode = model.PartnerSubscriptionCode,
            };

            var result = await Mediator.Send(query);
            if (!result)
            {
                return RedirectToAction("KYCReminderExpired");
            }
            else
            {

                return RedirectToAction("KYCReminderUnsubscribed");
            }
        }

        public IActionResult KYCReminderUnsubscribed()
        {
            return View();
        }

        public IActionResult KYCReminderExpired()
        {
            return View();
        }

        public IActionResult SuccessReset2FALink()
        {
            return View();
        }

        public IActionResult SuccessReset2FA()
        {
            return View();
        }

        public IActionResult ExpiredReset2FA()
        {
            return View();
        }
        public IActionResult AlreadyUsedReset2FA()
        {
            return View();
        }

        [HttpPost("[controller]/MFA/RequestReset")]
        public async Task<IActionResult> RequestResetMfa()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? await _userManager.GetUserAsync(User);

            RequestResetMfaCommand command = new RequestResetMfaCommand
            {
                ApplicationUser = user
            };

            Result result = await Mediator.Send(command);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError("{Command} failed to process! Error: {Error}", nameof(RequestResetMfaCommand), result.Error);

                return RedirectToAction("Login");
            }

            return View(nameof(SuccessReset2FALink));
        }

        [HttpGet("[controller]/MFA/Reset/{token}")]
        public async Task<IActionResult> ResetMfa([FromRoute] string token)
        {
            ResetMFAForRequestCommand command = new ResetMFAForRequestCommand
            {
                Token = token
            };

            Result<ResetMFAForRequestStatus> result = await Mediator.Send(command);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError("{Command} failed to process! Error: {Error}", nameof(ResetMFAForRequestCommand), result.Error);

                return RedirectToAction(nameof(LoginWith2fa));
            }

            switch (result.Value)
            {
                case ResetMFAForRequestStatus.Successful:
                    return RedirectToAction(nameof(SuccessReset2FA));
                case ResetMFAForRequestStatus.Expired:
                    return RedirectToAction(nameof(ExpiredReset2FA));
                case ResetMFAForRequestStatus.Used:
                    return RedirectToAction(nameof(AlreadyUsedReset2FA));
                default:
                    return RedirectToAction(nameof(ExpiredReset2FA));
            }
        }

        [HttpPost("[controller]/MFA/SetupSuccessful")]
        public async Task<IActionResult> SetupMfaSuccessful([FromServices] ResetMFASuccessfulEmailSender resetMFASuccessfulEmailSender,
            [FromServices] RegisterMfaSuccessfulEmailSender registerMFASuccessfulEmailSender)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            #region Marks existing user reset MFA flow completed
            bool isResetMFA = user.IsResetMFA ?? false;
            if (isResetMFA)
            {
                user.SetIsResetMFA(false);
                await _userManager.UpdateAsync(user);

                await resetMFASuccessfulEmailSender.NotifyUserAsync(user);
            }
            #endregion

            #region Marks new user setup MFA flow completed
            bool isNewUser = false;
            // Get claim from the current authentication ticket (`User`) instead of database (`UserManager.GetClaimsAsync`)
            System.Security.Claims.Claim isNewUserClaim = User.FindFirst(x => x.Type == TrangloUserManager.ClaimCode.IS_NEW_USER);
            if (isNewUserClaim != null 
                && bool.TryParse(isNewUserClaim.Value, out isNewUser)
                && isNewUser)
            {
                await _userManager.RemoveClaimAsync(user, isNewUserClaim);
                await _signInManager.RefreshSignInAsync(user);

                await registerMFASuccessfulEmailSender.NotifyUserAsync(user);
            }
            #endregion

            return NoContent();
        }
    }
}