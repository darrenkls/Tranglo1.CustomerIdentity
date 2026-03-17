using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Identity;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.PartnerUser, UACAction.Create)]
    public class LockUserCommand : IRequest<IdentityResult>
    {
        public string Email { get; set; }
    }

    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, IdentityResult>
    {
        private readonly TrangloUserManager _userManager;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly BusinessProfileService _businessProfileService;
        private readonly ILogger<LockUserCommandHandler> _logger;
		private readonly IBusinessProfileContext businessProfileContext;

		public LockUserCommandHandler(
                IBusinessProfileRepository businessProfileRepository,
                BusinessProfileService businessProfileService,
                TrangloUserManager userManager, 
                ILogger<LockUserCommandHandler> logger,
                IBusinessProfileContext businessProfileContext
            )
        {
            _businessProfileRepository = businessProfileRepository;
            _businessProfileService = businessProfileService;
            _userManager = userManager;
            _logger = logger;
			this.businessProfileContext = businessProfileContext;
		}

        public async Task<IdentityResult> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            CustomerUser applicationUser = await _userManager.FindByIdAsync(request.Email) as CustomerUser;
            if (applicationUser == null)
            {
                _logger.LogError("LockUser", $"Email: {request.Email} is not a valid user email.");
                return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = $"Email: {request.Email} is not a valid user email."
                        });
            }

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

			if (_ProfileMapping.CompanyUserBlockStatus == CompanyUserBlockStatus.Unblock)
			{
                _ProfileMapping.SetCompanyUserBlockStatus(CompanyUserBlockStatus.Block);
                
                await _businessProfileRepository.UpdateCustomerUserBusinessProfileAsync(_ProfileMapping, cancellationToken);
            }

            return IdentityResult.Success;

        }
    }
}
