using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.TrangloUser, UACAction.Edit)]
    [Permission(Permission.ManageAdminUser.Action_Deactivate_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageAdminUser.Action_View_Code })]
    public class UpdateTrangloStaffAccountStatusCommand : BaseCommand<Result<TrangloStaffAccountStatusOutputDTO>>
    {
        public string loginId { get; set; }
        public TrangloStaffAccountStatusInputDTO accountStatusInput { get; set; }
        public override Task<string> GetAuditLogAsync(Result<TrangloStaffAccountStatusOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                var status = Enumeration.FindById<AccountStatus>(accountStatusInput.AccountStatusCode);
                if (status.Equals(AccountStatus.Inactive)) {
                    return Task.FromResult("Deactivated User Account");
                }
            }

            return Task.FromResult<string>(null);
        }

        public class UpdateTrangloStaffAccountStatusCommandHandler : IRequestHandler<UpdateTrangloStaffAccountStatusCommand, Result<TrangloStaffAccountStatusOutputDTO>>
        {
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ITrangloRoleRepository _trangloRoleRepository;
            private readonly ILogger<UpdateTrangloStaffAccountStatusCommandHandler> _logger;
            public IUnitOfWork UnitOfWork { get; }

            public UpdateTrangloStaffAccountStatusCommandHandler(
                    IApplicationUserRepository applicationUserRepository,
                    ILogger<UpdateTrangloStaffAccountStatusCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    ITrangloRoleRepository trangloRoleRepository
                )
            {
                _applicationUserRepository = applicationUserRepository;
                _logger = logger;
                UnitOfWork = unitOfWork;
                _trangloRoleRepository = trangloRoleRepository;
            }
            public async Task<Result<TrangloStaffAccountStatusOutputDTO>> Handle(UpdateTrangloStaffAccountStatusCommand request, CancellationToken cancellationToken)
            {
                var trangloStaff = await _applicationUserRepository.GetTrangloUserByLoginId(request.loginId);
                if (trangloStaff == null)
                {
                    return Result.Failure<TrangloStaffAccountStatusOutputDTO>(
                           $"Tranglo Staff {request.loginId} Does Not Exist."
                       );
                }
                var accountStatus = Enumeration.FindById<AccountStatus>(request.accountStatusInput.AccountStatusCode);
                var trangloStaffAssignment = await _applicationUserRepository.GetTrangloStaffAssignment(request.loginId);
                
                if(accountStatus.Id == 1) { 

                    foreach(var profile in trangloStaffAssignment)
                    {
                        var trangloRoles = await _trangloRoleRepository.GetTrangloRoleByCodeAsync(profile.RoleCode);
                        var roleStatus = Enumeration.FindById<RoleStatus>(2);
                        if (trangloRoles.RoleStatus == roleStatus)
                        {
                           return Result.Failure<TrangloStaffAccountStatusOutputDTO>($"{trangloRoles.Description} role of this user has been deactivated. " +
                                $"Kindly change the user's role before reactivation.");
                        }
                    }
                }
                trangloStaff.SetAccountStatus(accountStatus);
                await _applicationUserRepository.SaveTrangloUserChanges(trangloStaff);

                TrangloStaffAccountStatusOutputDTO trangloStaffAccountStatusOutput = new TrangloStaffAccountStatusOutputDTO
                {
                    AccountStatusCode = trangloStaff.AccountStatusCode,
                    Description = accountStatus.Name
                };
                return Result.Success<TrangloStaffAccountStatusOutputDTO>(trangloStaffAccountStatusOutput);
            }
        }
    }
}
