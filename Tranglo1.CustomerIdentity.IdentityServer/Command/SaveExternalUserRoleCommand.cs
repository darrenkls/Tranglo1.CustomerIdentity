using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.ExternalUserRoleAggregate;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.PartnerManageExternalRole, UACAction.Create)]
    [Permission(Permission.ManageExternalRoles.Action_Add_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageExternalRoles.Action_View_Code })]
    internal class SaveExternalUserRoleCommand : BaseCommand<Result<NewExternalUserRoleOutputDTO>>
    {
        public NewExternalUserRoleInputDTO ExternalUserRoleInputDTO { get; set; }
        public override Task<System.String> GetAuditLogAsync(Result<NewExternalUserRoleOutputDTO> result)
        {
            if (result.IsSuccess) { 
            return Task.FromResult("Added new external role");
            }

            return base.GetAuditLogAsync(result);
        }
        public class SaveExternalUserRoleCommandHandler : IRequestHandler<SaveExternalUserRoleCommand, Result<NewExternalUserRoleOutputDTO>>
        {
            private readonly IExternalUserRoleRepository _externalUserRoleRepository;
            private readonly IPartnerRepository _partnerRepository;
            private readonly ILogger<SaveExternalUserRoleCommand> _logger;
            private readonly AccessControlManager _accessControlManager;
            public IUnitOfWork UnitOfWork { get; }

            public SaveExternalUserRoleCommandHandler(
                    ILogger<SaveExternalUserRoleCommand> logger,
                    IUnitOfWork unitOfWork,
                    AccessControlManager accessControlManager,
                    IExternalUserRoleRepository externalUserRoleRepository,
                    IPartnerRepository partnerRepository
                )
            {
                _logger = logger;
                UnitOfWork = unitOfWork;
                _accessControlManager = accessControlManager;
                _externalUserRoleRepository = externalUserRoleRepository;
                _partnerRepository = partnerRepository;
            }
            public async Task<Result<NewExternalUserRoleOutputDTO>> Handle(SaveExternalUserRoleCommand request, CancellationToken cancellationToken)
            {
                if(request.ExternalUserRoleInputDTO.PermissionInfoCodeList.Count() < 1)
                {
                    return Result.Failure<NewExternalUserRoleOutputDTO>(
                               $"Screen Access cannot be empty."
                           );
                }
                var getLatestRole = await _externalUserRoleRepository.GetLatestRoleCodeAsync();
                #region increment prefix

                string prefix = "EXT";
                string number;
                int i;
                string newPrefix;
                if (getLatestRole == null)
                {
                    number = "01";
                    newPrefix = prefix + number;
                }
                else
                {
                    number = Regex.Replace(getLatestRole.RoleCode, "^\\D+", "");
                    i = int.Parse(number) + 1;
                    newPrefix = prefix + i.ToString(new string('0', number.Length));
                }
                #endregion

                NewExternalUserRoleOutputDTO result = new NewExternalUserRoleOutputDTO();

                var solution = await _partnerRepository.GetSolutionAsync(request.ExternalUserRoleInputDTO.SolutionCode);
                var role = await _externalUserRoleRepository.GetExternalRoleByRoleCodeAsync(newPrefix);
                if (role == null)
                {
                    ExternalUserRole externalUser = new ExternalUserRole(request.ExternalUserRoleInputDTO.RoleName, newPrefix, solution);
                    
                    var addResult = await _externalUserRoleRepository.AddExternalUserRoleAsync(externalUser);
                     
                    if (addResult.IsFailure)
                    {
                        return Result.Failure<NewExternalUserRoleOutputDTO>(
                                $"Add External User Role failed for: ${newPrefix} "
                            );
                    }
                    foreach (var item in request.ExternalUserRoleInputDTO.PermissionInfoCodeList)
                    {
                        Dictionary<string, string> claims = new Dictionary<string, string>();
                        claims.Add("role", addResult.Value.RoleCode.ToLower());
                        var permissionInfo = _accessControlManager.GetPermissionInfo(item);

                        await _accessControlManager.Grant(claims, permissionInfo);
                        
                    }
                    result = new NewExternalUserRoleOutputDTO
                    {
                        RoleName = request.ExternalUserRoleInputDTO.RoleName,
                        SolutionCode = request.ExternalUserRoleInputDTO.SolutionCode,
                        PermissionInfoCodeList = request.ExternalUserRoleInputDTO.PermissionInfoCodeList
                    };
                    return Result.Success<NewExternalUserRoleOutputDTO>(result);
                }
                return Result.Failure<NewExternalUserRoleOutputDTO>(
                    $"External User Role Code {newPrefix} already exists"
                    );
            }
        }
    }
}
