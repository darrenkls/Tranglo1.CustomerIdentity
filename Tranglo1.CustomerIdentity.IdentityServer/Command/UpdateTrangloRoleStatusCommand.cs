using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.TrangloRole;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.TrangloRole, UACAction.Edit)]
    [Permission(Permission.ManageRoles.Action_EnableDisable,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageRoles.Action_View_Code })]
    internal class UpdateTrangloRoleStatusCommand : BaseCommand<Result<UpdateTrangloRoleStatusOutputDTO>>
    {
        public string RoleCode { get; set; }
        public UpdateTrangloRoleStatusInputDTO RoleStatusInputDTO { get; set; }
        public override Task<string> GetAuditLogAsync(Result<UpdateTrangloRoleStatusOutputDTO> result)
        {
            if (result.IsSuccess) 
            {
                var code = Enumeration.FindById<RoleStatus>(RoleStatusInputDTO.RoleStatusCode);
                if (code.Equals(RoleStatus.Active)) 
                {
                    return Task.FromResult("Enabled admin role");
                }

                if (code.Equals(RoleStatus.Inactive))
                {
                    return Task.FromResult("Disabled admin role");
                }
            }

            return base.GetAuditLogAsync(result);
        }

        public class UpdateTrangloRoleStatusCommandHandler : IRequestHandler<UpdateTrangloRoleStatusCommand, Result<UpdateTrangloRoleStatusOutputDTO>>
        {
            private readonly ITrangloRoleRepository _trangloRoleRepository;
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ILogger<UpdateTrangloRoleStatusCommandHandler> _logger;
            public IUnitOfWork UnitOfWork { get; }

            public UpdateTrangloRoleStatusCommandHandler(
                    ILogger<UpdateTrangloRoleStatusCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    IApplicationUserRepository applicationUserRepository,
                    ITrangloRoleRepository trangloRoleRepository
                )
            {
                _logger = logger;
                _applicationUserRepository = applicationUserRepository;
                UnitOfWork = unitOfWork;
                _trangloRoleRepository = trangloRoleRepository;
            }
            public async Task<Result<UpdateTrangloRoleStatusOutputDTO>> Handle(UpdateTrangloRoleStatusCommand request, CancellationToken cancellationToken)
            {
                var trangloRole = await _trangloRoleRepository.GetTrangloRoleByCodeAsync(request.RoleCode);
                if (trangloRole == null)
                {
                    return Result.Failure<UpdateTrangloRoleStatusOutputDTO>(
                           $"Tranglo Staff with Role {request.RoleCode} Does Not Exist."
                       );
                }

                var roleStatus = Enumeration.FindById<RoleStatus>(request.RoleStatusInputDTO.RoleStatusCode);
                if(roleStatus == null)
                {
                    return Result.Failure<UpdateTrangloRoleStatusOutputDTO>(
                           $"Role Status Code {request.RoleStatusInputDTO.RoleStatusCode} is Invalid."
                       );
                }
                if (request.RoleStatusInputDTO.RoleStatusCode == 1)
                {   
                    if(trangloRole.RoleStatus == roleStatus)
                    {
                        return Result.Failure<UpdateTrangloRoleStatusOutputDTO>(
                                        $"This role is currently active"
                                    );
                    }
                    trangloRole.RoleStatus = roleStatus;
                    
                }
                else if (request.RoleStatusInputDTO.RoleStatusCode == 2)
                {

                    var trangloRoleExists = await _applicationUserRepository.GetTrangloStaffAssignmentByRole(request.RoleCode);
                    foreach (var role in trangloRoleExists)
                    {
                        var trangloStaff = await _applicationUserRepository.GetApplicationUserByLoginId(role.LoginId);
                        if (trangloStaff.AccountStatus == AccountStatus.Active)
                        {
                            return Result.Failure<UpdateTrangloRoleStatusOutputDTO>(
                                        $"You are not able to disable this role as there are still active users under it "
                                    );
                        }
                    }
                    if (trangloRole.RoleStatus == roleStatus)
                    {
                        return Result.Failure<UpdateTrangloRoleStatusOutputDTO>(
                                        $"This role is currently inactive"
                                    );
                    }
                    trangloRole.RoleStatus = roleStatus;

                   
                }
                var result = await _trangloRoleRepository.UpdateTrangloRoleStatus(trangloRole);

                if (result.IsFailure)
                {
                    return Result.Failure<UpdateTrangloRoleStatusOutputDTO>(
                                $"Update Role Status Code failed for: ${request.RoleCode}, Role Status Code: ${request.RoleStatusInputDTO.RoleStatusCode} "
                            );
                }
                UpdateTrangloRoleStatusOutputDTO updateTrangloRoleStatusOutputDTO = new UpdateTrangloRoleStatusOutputDTO
                {
                    RoleStatusCode = roleStatus.Id
                };

                return Result.Success(updateTrangloRoleStatusOutputDTO);
            }
        }
    }
}
