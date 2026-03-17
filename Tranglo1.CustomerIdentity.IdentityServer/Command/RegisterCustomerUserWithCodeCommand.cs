using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
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
    public class RegisterCustomerUserWithCodeCommand : IRequest<IdentityResult>
    {
        public string RegistryCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RecaptchaToken { get; set; }
        public long PartnerRegistrationLeadsOriginCode { get; set; } = PartnerRegistrationLeadsOrigin.Website.Id;
        public string OtherLeadsOrigin { get; set; }
        public Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary ModelState { get; set; }
    }

    public class RegisterCustomerUserWithCodeCommandHandler : IRequestHandler<RegisterCustomerUserWithCodeCommand, IdentityResult>
    {
        private readonly TrangloUserManager _userManager;
        private readonly ICapchaValidator capchaValidator;
        private readonly INotificationService _notificationService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly ISignUpCodeRepository _signUpCodeRepository;
        private readonly IPartnerRepository _partnerRepository;

        public RegisterCustomerUserWithCodeCommandHandler(
            TrangloUserManager userManager,
            IApplicationUserRepository applicationUserRepository,
            ICapchaValidator capchaValidator,
            INotificationService notificationService,
            ISignUpCodeRepository signUpCodeRepository,
            IPartnerRepository partnerRepository)
        {
            _userManager = userManager;
            _applicationUserRepository = applicationUserRepository;
            _signUpCodeRepository = signUpCodeRepository;
            this.capchaValidator = capchaValidator;
            _notificationService = notificationService;
            _partnerRepository = partnerRepository;
        }

        public async Task<IdentityResult> Handle(RegisterCustomerUserWithCodeCommand request, CancellationToken cancellationToken)
        {
            var _recaptchaResult = await capchaValidator.ValidateAsync(request.RecaptchaToken);

            if (_recaptchaResult.IsFailure)
            {
                request.ModelState.AddModelError("RecaptchaToken", _recaptchaResult.Error);
            }

            var _RegistryCodeResult = RegistryCode.Create(request.RegistryCode);
            if (_RegistryCodeResult.IsFailure)
            {
                request.ModelState.AddModelError("RegistryCode", _RegistryCodeResult.Error);
            }

            var FindRegistryCode = await _signUpCodeRepository.GetSignUpCodesAsync(request.RegistryCode);
            if (FindRegistryCode == null)
            {
                request.ModelState.AddModelError("RegistryCode", "Invalid Registry Code.");
            }
            else if (FindRegistryCode.Status == SignUpAccountStatus.Expired || FindRegistryCode.ExpireAt < DateTime.UtcNow)
            {
                request.ModelState.AddModelError("RegistryCode", "This Registry Code is expired.");
            }
            else if (FindRegistryCode.Status == SignUpAccountStatus.Used)
            {
                request.ModelState.AddModelError("RegistryCode", "This Registry Code is used.");
            }

            var _EmailResult = Email.Create(request.Email);
            if (_EmailResult.IsFailure)
            {
                request.ModelState.AddModelError("Email", _EmailResult.Error);
            }

            var FindUserbyEmail = await _userManager.FindByIdAsync(request.Email);

            if (FindUserbyEmail != null)
            {
                request.ModelState.AddModelError("Email", $"Email {request.Email} already exists.");
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



            var signUpCodeInfo = await _signUpCodeRepository.GetSignUpCodesAsync(FindRegistryCode.Id);
            signUpCodeInfo.Status = SignUpAccountStatus.Used;
            await _signUpCodeRepository.UpdateSignUpCodesAsync(signUpCodeInfo);
            CustomerUser _Customer = null;
            var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionListAsync(signUpCodeInfo.PartnerCode);
            bool isTrangloConnect = partnerSubInfo.Any(x => x.Solution == Solution.Connect);
            bool isTrangloBusiness = partnerSubInfo.Any(x => x.Solution == Solution.Business);

            if (isTrangloConnect == true && isTrangloBusiness == true)
            {
                _Customer = CustomerUser.Register(FullName.Create(request.FullName).Value, _EmailResult.Value, request.Password, null, null, true);
            }
            else
            {
                if (isTrangloConnect == true)
                {
                    _Customer = CustomerUser.Register(FullName.Create(request.FullName).Value, _EmailResult.Value, request.Password, null, 1, false);
                }
                else if (isTrangloBusiness == true)
                {
                    _Customer = CustomerUser.Register(FullName.Create(request.FullName).Value, _EmailResult.Value, request.Password, null, 2, false);
                }
            }
            var RegisterUserResult = await _userManager.CreateAsync(_Customer, request.Password);

            PartnerRegistrationLeadsOrigin _leadsOrigin = Enumeration.FindById<PartnerRegistrationLeadsOrigin>(request.PartnerRegistrationLeadsOriginCode);

            PartnerRegistration partnerRegistration = await _partnerRepository.GetPartnerDetailsByCodeAsync(signUpCodeInfo.PartnerCode);

            var _CustomerUserRegistration = new CustomerUserRegistration(_EmailResult.Value, request.RegistryCode, FindRegistryCode.CompanyName, partnerRegistration.BusinessProfileCode,
                _leadsOrigin,
                request.OtherLeadsOrigin);
            await _applicationUserRepository.AddCustomerUserRegistration(_CustomerUserRegistration);

            return RegisterUserResult;
        }
    }
}
