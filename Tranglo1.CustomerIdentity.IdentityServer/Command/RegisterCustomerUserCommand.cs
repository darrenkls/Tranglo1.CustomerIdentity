using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha;

namespace Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands
{
    public class RegisterCustomerUserCommand : IRequest<IdentityResult>
    {
        public string CompanyName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? SolutionCode { get; set; }
        public int? CustomerTypeCode { get; set; }
        public bool HasSignUpCode { get; set; }
        public string CountryISO2 { get; set; }
        public string RecaptchaToken { get; set; }
        public string RegistryCode { get; set; }
        public long PartnerRegistrationLeadsOriginCode { get; set; } = PartnerRegistrationLeadsOrigin.Website.Id;
        public string OtherLeadsOrigin { get; set; }
        public Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary ModelState { get; set; }
    }

    public class RegisterCustomerUserCommandHandler : IRequestHandler<RegisterCustomerUserCommand, IdentityResult>
    {
        private readonly TrangloUserManager _userManager;
        private readonly ICapchaValidator capchaValidator;
        private readonly INotificationService _notificationService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly BusinessProfileService _businessProfileService;
        private readonly ISignUpCodeRepository _signUpCodeRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly ILogger<RegisterCustomerUserCommand> _logger;

        public RegisterCustomerUserCommandHandler(
            TrangloUserManager userManager,
            IApplicationUserRepository applicationUserRepository,
            BusinessProfileService businessProfileService,
            ICapchaValidator capchaValidator,
            INotificationService notificationService,
            ISignUpCodeRepository signUpCodeRepository,
            IPartnerRepository partnerRepository,
            IBusinessProfileRepository businessProfileRepository,
            ILogger<RegisterCustomerUserCommand> logger)
        {
            _userManager = userManager;
            _applicationUserRepository = applicationUserRepository;
            _businessProfileService = businessProfileService;
            this.capchaValidator = capchaValidator;
            _notificationService = notificationService;
            _logger = logger;
            _signUpCodeRepository = signUpCodeRepository;
            _partnerRepository = partnerRepository;
            _businessProfileRepository = businessProfileRepository;
        }

        public async Task<IdentityResult> Handle(RegisterCustomerUserCommand request, CancellationToken cancellationToken)
        {
            List<IdentityError> identityErrors = new List<IdentityError>();
            var _recaptchaResult = await capchaValidator.ValidateAsync(request.RecaptchaToken);
            if (_recaptchaResult.IsFailure)
            {
                //request.ModelState.AddModelError("RecaptchaToken", _recaptchaResult.Error);
                this.AddErrorMessage(identityErrors, "RecaptchaToken", _recaptchaResult.Error);
            }

            var _EmailResult = Email.Create(request.Email);
            if (_EmailResult.IsFailure)
            {
                //request.ModelState.AddModelError("Email", _EmailResult.Error);
                this.AddErrorMessage(identityErrors, "Email", _EmailResult.Error);
            }
            var FindUserbyEmail = await _userManager.FindByIdAsync(request.Email);

            if (FindUserbyEmail != null)
            {
                //request.ModelState.AddModelError("Email", $"Email {request.Email} already exists.");
                this.AddErrorMessage(identityErrors, "Email", $"Email {request.Email} already exists.");
            }

            PartnerRegistrationLeadsOrigin _leadsOrigin = Enumeration.FindById<PartnerRegistrationLeadsOrigin>(request.PartnerRegistrationLeadsOriginCode);

            if (request.HasSignUpCode)
            {
                var _RegistryCodeResult = RegistryCode.Create(request.RegistryCode);
                if (_RegistryCodeResult.IsFailure)
                {
                    //request.ModelState.AddModelError("RegistryCode", _RegistryCodeResult.Error);
                    this.AddErrorMessage(identityErrors, "RegistryCode", _RegistryCodeResult.Error);
                }

                var FindRegistryCode = await _signUpCodeRepository.GetSignUpCodesAsync(request.RegistryCode);
                if (FindRegistryCode == null)
                {
                    //request.ModelState.AddModelError("RegistryCode", "Invalid Registry Code.");
                    this.AddErrorMessage(identityErrors, "RegistryCode", "Invalid Registry Code.");
                }
                else if (FindRegistryCode.Status == SignUpAccountStatus.Expired || FindRegistryCode.ExpireAt < DateTime.UtcNow)
                {
                    // request.ModelState.AddModelError("RegistryCode", "This Registry Code is expired.");
                    this.AddErrorMessage(identityErrors, "RegistryCode", "This Registry Code is expired.");
                }
                else if (FindRegistryCode.Status == SignUpAccountStatus.Used)
                {
                    //request.ModelState.AddModelError("RegistryCode", "This Registry Code is used.");
                    this.AddErrorMessage(identityErrors, "RegistryCode", "This Registry Code is used.");
                }

                IdentityError[] identityErrorsAB = identityErrors.ToArray();
                if (identityErrors.Count > 0)
                {
                    return IdentityResult.Failed(identityErrorsAB);
                }

                //Get the details on country and PIC from sign up code via partner/business profile
                var signUpCodeInfo = await _signUpCodeRepository.GetSignUpCodesAsync(FindRegistryCode.Id);

                var partner = await _partnerRepository.GetPartnerRegistrationByCodeAsync(signUpCodeInfo.PartnerCode);
                if (partner is null)
                {
                    this.AddErrorMessage(identityErrors, "RegistryCode", "No Partner is tied to the sign up code");
                    IdentityError[] identityErrorsP = identityErrors.ToArray();
                    if (identityErrors.Count > 0)
                    {
                        return IdentityResult.Failed(identityErrorsP);
                    }
                }

                var businessProfile = await _businessProfileRepository.GetBusinessProfileByCodeAsync(partner.BusinessProfileCode);
                var _CustomerUserRegistrationRegistryCode = new CustomerUserRegistration(_EmailResult.Value, request.RegistryCode, FindRegistryCode.CompanyName, businessProfile.Id, _leadsOrigin, request.OtherLeadsOrigin);
                await _applicationUserRepository.AddCustomerUserRegistration(_CustomerUserRegistrationRegistryCode);

                signUpCodeInfo.Status = SignUpAccountStatus.Used;
                await _signUpCodeRepository.UpdateSignUpCodesAsync(signUpCodeInfo);

                var fullNameResult = FullName.Create(request.FullName);

                if (partner.CustomerTypeCode == CustomerType.Individual.Id)
                {
                    fullNameResult = FullName.Create(businessProfile.ContactPersonName); //Name PIC field points to ContactPersonName
                }

                if (fullNameResult.IsFailure)
                {
                    this.AddErrorMessage(identityErrors, "FullName", "Unable to construct P-I-C Name");
                    IdentityError[] identityErrorsP = identityErrors.ToArray();
                    if (identityErrors.Count > 0)
                    {
                        return IdentityResult.Failed(identityErrorsP);
                    }
                }
                CustomerUser _CustomerRegistry = null;
                var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionListAsync(signUpCodeInfo.PartnerCode);
                bool isTrangloConnect = partnerSubInfo.Any(x => x.Solution == Solution.Connect);
                bool isTrangloBusiness = partnerSubInfo.Any(x => x.Solution == Solution.Business);


                if (isTrangloConnect == true && isTrangloBusiness == true)
                {
                    _CustomerRegistry = CustomerUser.Register(fullNameResult.Value, _EmailResult.Value, request.Password, null, request.SolutionCode, true);
                }
                else
                {
                    if (isTrangloConnect == true)
                    {
                        _CustomerRegistry = CustomerUser.Register(fullNameResult.Value, _EmailResult.Value, request.Password, null, 1, false);
                    }
                    else if (isTrangloBusiness == true)
                    {
                        _CustomerRegistry = CustomerUser.Register(fullNameResult.Value, _EmailResult.Value, request.Password, null, 2, false);
                    }
                }

                var registerUserRegistryResult = await _userManager.CreateAsync(_CustomerRegistry, request.Password);

                return registerUserRegistryResult;
            }

            if (request.CompanyName is null)
            {
                request.CompanyName = "";
            }
            if (request.CustomerTypeCode == CustomerType.Individual.Id)
            {
                request.FullName = request.CompanyName;
            }
            var _isExistingCompanyName = await _businessProfileService.CheckIsExistingCompanyNameAsync(request.CompanyName);

            if (_isExistingCompanyName.isInUsed && request.CompanyName != "")
            {
                //request.ModelState.AddModelError("CompanyName", "Company Name already exists. Please input a unique naming.");
                this.AddErrorMessage(identityErrors, "CompanyName", "Company Name already exists. Please input a unique naming.");
            }

            //to retrieve from sign up code
            var _FindCountry = CountryMeta.GetCountryByISO2Async(request.CountryISO2);

            CountryMeta country = _FindCountry;
            if (request.CountryISO2 != null && country == null)
            {
                // request.ModelState.AddModelError("CountryISO2", "Invalid country code [{request.CountryISO2}]");
                IdentityError identityError = new IdentityError();
                this.AddErrorMessage(identityErrors, "CountryISO2", $"Invalid country code [{request.CountryISO2}]");
            }

            if (!request.ModelState.IsValid)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "500",
                        Description = "Registration Validation Failure"
                    });
            }

            IdentityError[] identityErrorsA = identityErrors.ToArray();
            if (identityErrors.Count > 0)
            {
                return IdentityResult.Failed(identityErrorsA);
            }


            if (request.SolutionCode == Solution.Connect.Id)
            {
                request.CustomerTypeCode = (int)CustomerType.Remittance_Partner.Id;
            }

            if (request.SolutionCode == Solution.Business.Id)
            {
                request.CustomerTypeCode = (int)CustomerType.Corporate_Normal_Corporate.Id;
            }

            var _CustomerUserRegistration = new CustomerUserRegistration(_EmailResult.Value, CompanyName.Create(request.CompanyName).Value, request.SolutionCode, request.CustomerTypeCode,
                _leadsOrigin,
                request.OtherLeadsOrigin);
            await _applicationUserRepository.AddCustomerUserRegistration(_CustomerUserRegistration);

            var _Customer = CustomerUser.Register(FullName.Create(request.FullName).Value, _EmailResult.Value, request.Password, country, request.SolutionCode, false);
            var RegisterUserResult = await _userManager.CreateAsync(_Customer, request.Password);

            return RegisterUserResult;
        }

        private void AddErrorMessage(List<IdentityError> identityErrors, string code, string description)
        {
            IdentityError identityError = new IdentityError();
            identityError.Code = code;
            identityError.Description = description;

            identityErrors.Add(identityError);
        }
    }
}
