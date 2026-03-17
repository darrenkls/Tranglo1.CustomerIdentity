using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
using Action = Tranglo1.UserAccessControl.UACAction;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.TrangloRole, UACAction.Create)]
    [Permission(Permission.ManageRoles.Action_Add_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageRoles.Action_View_Code })]
    internal class AddTrangloRoleCommand : BaseCommand<Result<AddTrangloRoleOutputDTO>>
    {
        public string CreatorRoleCode { get; set; }
        public AddTrangloRoleInputDTO AddTrangloRoleInput { get; set; }
        public override Task<string> GetAuditLogAsync(Result<AddTrangloRoleOutputDTO> result)
        {
            if (result.IsSuccess) 
            {
                return Task.FromResult("Added new admin role");
            }

            return base.GetAuditLogAsync(result);
        }
        public class AddTrangloRoleCommandHandler : IRequestHandler<AddTrangloRoleCommand, Result<AddTrangloRoleOutputDTO>>
        {
            private readonly ITrangloRoleRepository _trangloRoleRepository;
            private readonly ILogger<AddTrangloRoleCommandHandler> _logger;
            private readonly AccessControlManager _accessControlManager;
            public IUnitOfWork UnitOfWork { get; }

            public AddTrangloRoleCommandHandler(
                    ILogger<AddTrangloRoleCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    AccessControlManager accessControlManager,
                    ITrangloRoleRepository trangloRoleRepository
                )
            {
                _logger = logger;
                UnitOfWork = unitOfWork;
                _accessControlManager = accessControlManager;
                _trangloRoleRepository = trangloRoleRepository;
            }

            public async Task<Result<AddTrangloRoleOutputDTO>> Handle(AddTrangloRoleCommand request, CancellationToken cancellationToken)
            {
                if(request.AddTrangloRoleInput.PermissionInfoCodeList.Count() < 1)
                {
                    return Result.Failure<AddTrangloRoleOutputDTO>(
                        $"Screen Access cannot be empty."                      
                    );
                }
                var getLatestRole = await _trangloRoleRepository.GetLatestRoleByDept(request.AddTrangloRoleInput.TrangloDepartmentCode);
                #region increment prefix

                string prefix;
                string number;
                int i;
                string newPrefix;
                if (getLatestRole == null)
                {
                    prefix = Enumeration.FindById<TrangloDepartment>(request.AddTrangloRoleInput.TrangloDepartmentCode).DepartmentPrefix;
                    number = "01";
                    newPrefix = prefix + number;
                }
                else
                {
                    prefix = Regex.Match(getLatestRole.Id, "^\\D+").Value;
                    number = Regex.Replace(getLatestRole.Id, "^\\D+", "");
                    i = int.Parse(number) + 1;
                    newPrefix = prefix + i.ToString(new string('0', number.Length));
                }
                #endregion

                AddTrangloRoleOutputDTO result = new AddTrangloRoleOutputDTO();
 
                #region Add
                var role = await _trangloRoleRepository.GetTrangloRoleByCodeAsync(newPrefix);
                TrangloDepartment department = Enumeration.FindById<TrangloDepartment>(request.AddTrangloRoleInput.TrangloDepartmentCode);
                AuthorityLevel authorityLevel = Enumeration.FindById<AuthorityLevel>(request.AddTrangloRoleInput.AuthorityLevelCode);
                if (role == null)
                {
                    TrangloRole trangloRole = new TrangloRole(newPrefix.ToUpper(), request.AddTrangloRoleInput.RoleDescription,
                                                 department, authorityLevel, request.CreatorRoleCode);
                    trangloRole.IsSuperApprover = request.AddTrangloRoleInput.IsSuperApprover;
                    var addResult = await _trangloRoleRepository.AddTrangloRole(trangloRole);

                    if (addResult.IsFailure)
                    {
                        return Result.Failure<AddTrangloRoleOutputDTO>(
                                $"Add Tranglo Role failed for: ${newPrefix} "
                            );
                    }
                    foreach (var item in request.AddTrangloRoleInput.PermissionInfoCodeList)
                    {
                        Dictionary<string, string> claims = new Dictionary<string, string>();
                        claims.Add("role", addResult.Value.Id.ToLower());
                        var permissionInfo = _accessControlManager.GetPermissionInfo(item);
                        
                        await _accessControlManager.Grant(claims, permissionInfo);
                    }
                    result = new AddTrangloRoleOutputDTO
                    {
                        AuthorityLevelCode = trangloRole.AuthorityLevel.Id,
                        DepartmentCode = trangloRole.TrangloDepartment.Id,
                        IsSuperApprover = trangloRole.IsSuperApprover,
                        RoleName = trangloRole.Description,
                        PermissionInfoCodeList = request.AddTrangloRoleInput.PermissionInfoCodeList
                    };
                    return Result.Success<AddTrangloRoleOutputDTO>(result);
                }
                return Result.Failure<AddTrangloRoleOutputDTO>(
                    $"Tranglo Role Code {newPrefix} already exists"
                    );
                #endregion
            }

        }
    }
}
