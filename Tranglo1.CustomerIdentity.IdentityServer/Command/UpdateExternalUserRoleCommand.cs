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
using Tranglo1.Common.Cache;
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
    [Permission(Permission.ManageExternalRoles.Action_Edit_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageExternalRoles.Action_View_Code })]
    internal class UpdateExternalUserRoleCommand : BaseCommand<Result<UpdateExternalUserRoleOutputDTO>>
    {
        public string RoleCode { get; set; }
        public UpdateExternalUserRoleInputDTO UpdateExternalUserRoleInputDTO { get; set; }
        public override Task<System.String> GetAuditLogAsync(Result<UpdateExternalUserRoleOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                return Task.FromResult("Edited external role details");
            }

            return base.GetAuditLogAsync(result);
        }
        public class UpdateExternalUserRoleCommandHandler : IRequestHandler<UpdateExternalUserRoleCommand, Result<UpdateExternalUserRoleOutputDTO>>
        {
            private readonly IExternalUserRoleRepository _externalUserRoleRepository;
            private readonly ILogger<UpdateExternalUserRoleCommandHandler> _logger;
            private readonly AccessControlManager _accessControlManager;
            private readonly IRedisCacheManager _redis;

            public IUnitOfWork UnitOfWork { get; }

            public UpdateExternalUserRoleCommandHandler(
                    ILogger<UpdateExternalUserRoleCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    AccessControlManager accessControlManager,
                    IExternalUserRoleRepository externalUserRoleRepository, IRedisCacheManager redis)
            {
                _logger = logger;
                UnitOfWork = unitOfWork;
                _accessControlManager = accessControlManager;
                _externalUserRoleRepository = externalUserRoleRepository;
                _redis = redis;
            }
            public async Task<Result<UpdateExternalUserRoleOutputDTO>> Handle(UpdateExternalUserRoleCommand request, CancellationToken cancellationToken)
            {
                var externalRole = await _externalUserRoleRepository.GetExternalRoleByRoleCodeAsync(request.RoleCode);
                if (externalRole == null)
                {
                    return Result.Failure<UpdateExternalUserRoleOutputDTO>(
                                $"External Role Code: ${request.RoleCode} does not exist."
                            );
                }

                foreach (var item in request.UpdateExternalUserRoleInputDTO.PermissionInfoList)
                {

                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("role", request.RoleCode);

                    var permissionInfo = _accessControlManager.GetPermissionInfo(item.PermissionInfoCode);
                    var hasPermission = _accessControlManager.HasPermission(keyValuePairs, permissionInfo);

                    if (item.IsSelected)
                    {
                        if (hasPermission == false)
                        {
                            await _accessControlManager.Grant(keyValuePairs, permissionInfo);
                        }
                    }
                    else
                    {
                        if (hasPermission == true)
                        {
                            await _accessControlManager.Revoke(keyValuePairs, permissionInfo);
                        }

                    }
                }

                //Remove Login Access Cache && Permission List Cache
                _redis.RemoveCache($"LoginAccess_{request.RoleCode}");
                _redis.RemoveCache($"PermissionAssignments_{request.RoleCode}");

                UpdateExternalUserRoleOutputDTO result = new UpdateExternalUserRoleOutputDTO
                {
                    PermissionInfoList = request.UpdateExternalUserRoleInputDTO.PermissionInfoList
                };
                return result;
            }
        }
    }
}
