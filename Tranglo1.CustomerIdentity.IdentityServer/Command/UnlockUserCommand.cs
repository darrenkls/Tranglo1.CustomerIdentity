using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Identity;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class UnlockUserCommand : IRequest<IdentityResult>
    {
        public string Email { get; set; }
    }

    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, IdentityResult>
    {
        private readonly TrangloUserManager _userManager;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly BusinessProfileService _businessProfileService;
        private readonly ILogger<UnlockUserCommandHandler> _logger;
		private readonly IBusinessProfileContext businessProfileContext;

		public UnlockUserCommandHandler(
                IBusinessProfileRepository businessProfileRepository,
                BusinessProfileService businessProfileService,
                TrangloUserManager userManager,
                ILogger<UnlockUserCommandHandler> logger,
                IBusinessProfileContext businessProfileContext
            )
        {
            _businessProfileRepository = businessProfileRepository;
            _businessProfileService = businessProfileService;
            _userManager = userManager;
            _logger = logger;
			this.businessProfileContext = businessProfileContext;
		}

        public async Task<IdentityResult> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            var _CurrentProfile = businessProfileContext.CurrentProfileId;

            if (_CurrentProfile.HasNoValue)
            {
                //Caution: We will reach here if the caller is from Tranglo Admin
                _logger.LogError($"Unable to determine business profile. Received email: [{request.Email}]");

                return IdentityResult.Failed(new IdentityError()
                {
                    Description = "Unable to determine business profile."
                });
            }
            
            CustomerUser applicationUser = await _userManager.FindByIdAsync(request.Email) as CustomerUser;
            if (applicationUser == null)
            {
                _logger.LogError("UnlockUser", $"Email: {request.Email} is not a valid user email.");
                return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = $"Email: {request.Email} is not a valid user email."
                        });
            }

            if (applicationUser.AccountStatus != AccountStatus.Blocked)
			{
                return IdentityResult.Failed(new IdentityError()
                {
                    Description = $"Account [{applicationUser.LoginId}] is not in Blocked state."
                });
			}

            IdentityResult ResetAccessFailedCountResponse = await _userManager.ResetAccessFailedCountAsync(applicationUser);
            
            if (!ResetAccessFailedCountResponse.Succeeded)
            {
                _logger.LogError("ResetAccessFailedCount", ResetAccessFailedCountResponse.Errors);
                return ResetAccessFailedCountResponse;
            }

			IdentityResult SetLockoutEndDateResponse = await _userManager.SetLockoutEndDateAsync(applicationUser, null);
            if (!SetLockoutEndDateResponse.Succeeded)
            {
                _logger.LogError("SetLockoutEndDate", SetLockoutEndDateResponse.Errors);
                return SetLockoutEndDateResponse;
            }

            var _Profiles = await _businessProfileService
                .GetCustomerUserBusinessProfilesAsync(applicationUser, _CurrentProfile.Value);

            if (_Profiles.IsFailure || _Profiles.Value.Count == 0)
            {
                _logger.LogError($"The given user [{request.Email}] does not matched with business profile.");

                return IdentityResult.Failed(new IdentityError()
                {
                    Description = $"The given user [{request.Email}] does not matched with business profile."
                });
            }

            //Note: Should be only one profile mapping per company
            var _ProfileMapping = _Profiles.Value.First();

            if (_Profiles.Value.Count > 1)
            {
                _logger.LogCritical($"More than one business profile record was found for [{request.Email}]. Current profile Id = [{_CurrentProfile.Value}]");
            }

			if (_ProfileMapping.CompanyUserBlockStatus == CompanyUserBlockStatus.Block)
            {
                _ProfileMapping.SetCompanyUserBlockStatus(CompanyUserBlockStatus.Unblock);
                await _businessProfileRepository.UpdateCustomerUserBusinessProfileAsync(_ProfileMapping, cancellationToken);
            }

            return IdentityResult.Success;
        }
    }
}
