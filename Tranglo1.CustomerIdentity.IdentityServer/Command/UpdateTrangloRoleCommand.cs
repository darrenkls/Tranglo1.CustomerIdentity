using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Tranglo1.Common.Cache;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.TrangloRole, UACAction.Edit)]
    [Permission(Permission.ManageRoles.Action_Edit_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageRoles.Action_View_Code })]
    internal class UpdateTrangloRoleCommand : BaseCommand<Result<UpdateTrangloRoleOutputDTO>>
    {
        public UpdateTrangloRoleInputDTO UpdateTrangloRole { get; set; }
        public string RoleCode { get; set; }
        public override Task<string> GetAuditLogAsync(Result<UpdateTrangloRoleOutputDTO> result)
        {
            if (result.IsSuccess) 
            {
                return Task.FromResult("Edited admin role");
            }

            return base.GetAuditLogAsync(result);
        }
        public class UpdateTrangloRoleCommandHandler : IRequestHandler<UpdateTrangloRoleCommand, Result<UpdateTrangloRoleOutputDTO>>
        {
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ITrangloRoleRepository _trangloRoleRepository;
            private readonly ILogger<UpdateTrangloRoleCommandHandler> _logger;
            private readonly AccessControlManager _accessControlManager;
            private readonly IRedisCacheManager _redis;

            public IUnitOfWork UnitOfWork { get; }

            public UpdateTrangloRoleCommandHandler(
                    IApplicationUserRepository applicationUserRepository,
                    ILogger<UpdateTrangloRoleCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    ITrangloRoleRepository trangloRoleRepository,
                    AccessControlManager accessControlManager, IRedisCacheManager redis)
            {
                _applicationUserRepository = applicationUserRepository;
                _logger = logger;
                UnitOfWork = unitOfWork;
                _accessControlManager = accessControlManager;
                _trangloRoleRepository = trangloRoleRepository;
                _redis = redis;
            }

            public async Task<Result<UpdateTrangloRoleOutputDTO>> Handle(UpdateTrangloRoleCommand request, CancellationToken cancellationToken)
            {
                var trangloRole = await _trangloRoleRepository.GetTrangloRoleByCodeAsync(request.RoleCode);
                trangloRole.IsSuperApprover = request.UpdateTrangloRole.IsSuperApprover;
                var updateSuperApprover = await _trangloRoleRepository.UpdateTrangloRole(trangloRole);

                if (updateSuperApprover.IsFailure)
                {
                    return Result.Failure<UpdateTrangloRoleOutputDTO>(
                                $"Update Role Code failed for: ${request.RoleCode} "
                            );
                }
                foreach (var item in request.UpdateTrangloRole.PermissionInfoList) {

                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("role", request.RoleCode);

                    var permissionInfo = _accessControlManager.GetPermissionInfo(item.PermissionInfoCode);
                    var hasPermission = _accessControlManager.HasPermission(keyValuePairs, permissionInfo);

                    if (item.IsSelected)
                    {
                        if(hasPermission == false)
                        {
                            await _accessControlManager.Grant(keyValuePairs, permissionInfo);
                        }
                    }
                    else
                    {
                        if(hasPermission == true)
                        {
                            await _accessControlManager.Revoke(keyValuePairs, permissionInfo);
                        }
                        
                    }
                }

                //Remove Login Access Cache && Permission List Cache
                _redis.RemoveCache($"LoginAccess_{request.RoleCode}");
                _redis.RemoveCache($"PermissionAssignments_{request.RoleCode}");

                UpdateTrangloRoleOutputDTO result = new UpdateTrangloRoleOutputDTO
                {
                    IsSuperApprover = request.UpdateTrangloRole.IsSuperApprover,
                    PermissionInfoList = request.UpdateTrangloRole.PermissionInfoList
                };
                return result;
            }
        }
    }
}
