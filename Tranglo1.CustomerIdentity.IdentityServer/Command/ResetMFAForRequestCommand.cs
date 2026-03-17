using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.MFA;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class ResetMFAForRequestCommand : BaseCommand<Result<ResetMFAForRequestStatus>>
    {
        public string Token { get; set; }

        public class ResetMFAForRequestCommandHandler : IRequestHandler<ResetMFAForRequestCommand, Result<ResetMFAForRequestStatus>>
        {
            private readonly TrangloUserManager _userManager;
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ILogger<ResetMFAForRequestCommandHandler> _logger;
            private readonly IConfiguration _configuration;

            public ResetMFAForRequestCommandHandler(TrangloUserManager userManager,
                IApplicationUserRepository applicationUserRepository,
                ILogger<ResetMFAForRequestCommandHandler> logger,
                IConfiguration configuration)
            {
                _userManager = userManager;
                _applicationUserRepository = applicationUserRepository;
                _logger = logger;
                _configuration = configuration;
            }

            public async Task<Result<ResetMFAForRequestStatus>> Handle(ResetMFAForRequestCommand request, CancellationToken cancellationToken)
            {
                int resetMFALinkValidInMinutes = _configuration.GetValue<int>("ResetMFALinkValidInMinutes");

                var resetMFARequest = await _applicationUserRepository.GetMFAResetRequestByTokenAsync(request.Token);
                if (resetMFARequest == null || (resetMFARequest.CreatedDate.AddMinutes(resetMFALinkValidInMinutes) < DateTime.UtcNow))
                {
                    return Result.Success(ResetMFAForRequestStatus.Expired);
                }

                if (resetMFARequest.IsUsed)
                {
                    return Result.Success(ResetMFAForRequestStatus.Used);
                }

                var user = resetMFARequest.User;

                #region Update MFAResetRequest entry
                resetMFARequest.IsUsed = true;

                await _applicationUserRepository.UpdateMFAResetRequestAsync(resetMFARequest);
                #endregion Update MFAResetRequest entry

                #region Disable 2FA
                var hasEnabled2FA = await _userManager.GetTwoFactorEnabledAsync(user);
                if (hasEnabled2FA)
                {
                    await _applicationUserRepository.SetMFAAsync(user, AuthenticationType.Email, null, null);
                }
                #endregion

                await _applicationUserRepository.SetIsResetMFAAsync(user, true);

                return Result.Success(ResetMFAForRequestStatus.Successful);
            }
        }
    }
}
