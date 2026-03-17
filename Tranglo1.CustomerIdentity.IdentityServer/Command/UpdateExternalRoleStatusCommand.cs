using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.PartnerManageExternalRole, UACAction.Edit)]
    [Permission(Permission.ManageExternalRoles.Action_Disable_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageExternalRoles.Action_View_Code })]
    internal class UpdateExternalRoleStatusCommand : BaseCommand<Result<UpdateExternalRoleStatusOutputDTO>>
    {
        public string RoleCode { get; set; }
        public UpdateExternalRoleStatusInputDTO UpdateExternalRoleStatusInputDTO { get; set; }
        public override Task<System.String> GetAuditLogAsync(Result<UpdateExternalRoleStatusOutputDTO> result)
        {
            if (result.IsSuccess) 
            {
                var roleStatus = Enumeration.FindById<ExternalUserRoleStatus>(UpdateExternalRoleStatusInputDTO.RoleStatusCode);
                if (roleStatus.Equals(ExternalUserRoleStatus.Active)) 
                {
                    //return Task.FromResult("Enabled external role");
                }

                if (roleStatus.Equals(ExternalUserRoleStatus.Inactive)) 
                {
                    return Task.FromResult("Disabled external role");
                }
            }

            return base.GetAuditLogAsync(result);
        }
        public class UpdateExternalRoleStatusCommandHandler : IRequestHandler<UpdateExternalRoleStatusCommand, Result<UpdateExternalRoleStatusOutputDTO>>
        {
            private readonly IExternalUserRoleRepository _externalUserRoleRepository;
            private readonly ILogger<UpdateExternalRoleStatusCommandHandler> _logger;
            private readonly IBusinessProfileRepository _businessProfileRepository;
            public IUnitOfWork UnitOfWork { get; }

            public UpdateExternalRoleStatusCommandHandler(
                    ILogger<UpdateExternalRoleStatusCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    IBusinessProfileRepository businessProfileRepository,
                    IExternalUserRoleRepository externalUserRoleRepository
                )
            {
                _logger = logger;
                _businessProfileRepository = businessProfileRepository;
                UnitOfWork = unitOfWork;
                _externalUserRoleRepository = externalUserRoleRepository;
            }
            public async Task<Result<UpdateExternalRoleStatusOutputDTO>> Handle(UpdateExternalRoleStatusCommand request, CancellationToken cancellationToken)
            {
                var externalRole = await _externalUserRoleRepository.GetExternalRoleByRoleCodeAsync(request.RoleCode);
                if (externalRole == null)
                {
                    return Result.Failure<UpdateExternalRoleStatusOutputDTO>(
                           $"External User Role {request.RoleCode} Does Not Exist."
                       );
                }
                var roleStatus = Enumeration.FindById<ExternalUserRoleStatus>(request.UpdateExternalRoleStatusInputDTO.RoleStatusCode);
                if (roleStatus == null)
                {
                    return Result.Failure<UpdateExternalRoleStatusOutputDTO>(
                           $"Role Status Code {request.UpdateExternalRoleStatusInputDTO.RoleStatusCode} is Invalid."
                       );
                }
                if (request.UpdateExternalRoleStatusInputDTO.RoleStatusCode == 1)
                {
                    if (externalRole.ExternalUserRoleStatus == roleStatus)
                    {
                        return Result.Failure<UpdateExternalRoleStatusOutputDTO>(
                                        $"This role is currently active"
                                    );
                    }
                    externalRole.SetRoleStatusCode(roleStatus);

                }
                else if (request.UpdateExternalRoleStatusInputDTO.RoleStatusCode == 2)
                {
                    var externalRoleExist =await _businessProfileRepository.GetCustomerUserBusinessProfilesByRoleCodeAsync(request.RoleCode);
                    if(externalRoleExist.Count > 0)
                    {
                        return Result.Failure<UpdateExternalRoleStatusOutputDTO>(
                                      $"You are not able to disable this role as there are still active users under it "
                                  );
                    }
                    if (externalRole.ExternalUserRoleStatus == roleStatus)
                    {
                        return Result.Failure<UpdateExternalRoleStatusOutputDTO>(
                                        $"This role is currently inactive"
                                    );
                    }
                    externalRole.SetRoleStatusCode(roleStatus);    
                }
                var result = await _externalUserRoleRepository.UpdateExternalUserRoleStatusAsync(externalRole);

                if (result.IsFailure)
                {
                    return Result.Failure<UpdateExternalRoleStatusOutputDTO>(
                                $"Update Role Status Code failed for: ${request.RoleCode}, Role Status Code: ${request.UpdateExternalRoleStatusInputDTO.RoleStatusCode} "
                            );
                }
                UpdateExternalRoleStatusOutputDTO updateExternalRoleStatusOutputDTO = new UpdateExternalRoleStatusOutputDTO
                {
                    RoleStatusCode = roleStatus.Id
                };
                return Result.Success(updateExternalRoleStatusOutputDTO);
            }
        }
    }
}
