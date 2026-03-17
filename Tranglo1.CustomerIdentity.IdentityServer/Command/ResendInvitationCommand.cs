using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.PartnerUser, UACAction.Create)]
    [Permission(Permission.ManagePartnerUser.Action_ResendVerificationEmail_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManagePartnerUser.Action_View_Code, Permission.ManagePartnerUser.Action_ViewDetail_Code })]
    public class ResendInvitationCommand : IRequest<Result>
    {
        public int BusinessProfileCode { get; set; }
        public string InviterEmail { get; set; }
        public string InviteeEmail { get; set; }
        public string LoginId { get; set;}

    }
    public class ResendInvitationCommandHandler : IRequestHandler<ResendInvitationCommand, Result>
    {
        private readonly TrangloUserManager _userManager;
        private readonly ILogger<ResendInvitationCommandHandler> _logger;
        private readonly IBusinessProfileRepository _businessProfile;
        public ResendInvitationCommandHandler(
            TrangloUserManager userManager,
            ILogger<ResendInvitationCommandHandler> logger,
            IBusinessProfileRepository businessProfile
            )
        {
            _businessProfile = businessProfile;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result> Handle(ResendInvitationCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser inviter = await _userManager.FindByIdAsync(request.LoginId) ;
            var customerUser = await _userManager.FindByIdAsync(request.InviteeEmail);
            if (customerUser != null)
            {
                if(customerUser.AccountStatus == AccountStatus.PendingActivation) { 
                    var inviteUserResult = await _userManager.ResendInvitationAsync(
                                                           inviter,
                                                           request.BusinessProfileCode,
                                                           customerUser.Email
                                                       );

                    if (inviteUserResult.IsFailure)
                    {

                        _logger.LogError($"[ResendInvitationCommand] Invite user [{customerUser.Email}] failed. {inviteUserResult.Error}.");
                        return Result.Failure($"Invite user[{customerUser.Email}] failed. {inviteUserResult.Error}.");

                    }
                }
                else
                {
                    return Result.Failure($"Invite user[{customerUser.Email}] failed. User account is not pending for activation");
                }
            }
            else
            {
                return Result.Failure($"Invite user[{customerUser.Email}] failed. User does not exist.");
            }
            return Result.Success();
        }
    }
}
