using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.TrangloRole;
using Tranglo1.CustomerIdentity.IdentityServer.Helper.ACL;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.TrangloRole, UACAction.View)]
    [Permission(Permission.ManageRoles.Action_View_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { })]
    internal class GetTrangloRoleByRoleCodeQuery : IRequest<Result<GetTrangloRoleByRoleCodeOutputDTO>>
    {
        public string RoleCode { get; set; }

        public class GetTrangloRoleByRoleCodeQueryHandler : IRequestHandler<GetTrangloRoleByRoleCodeQuery, Result<GetTrangloRoleByRoleCodeOutputDTO>>
        {
            private readonly IConfiguration _config;
            private AccessControlManager _accessControlManager;
            public GetTrangloRoleByRoleCodeQueryHandler(IConfiguration config, AccessControlManager accessControlManager)
            {
                _config = config;
                _accessControlManager = accessControlManager;
            }

            public async Task<Result<GetTrangloRoleByRoleCodeOutputDTO>> Handle(GetTrangloRoleByRoleCodeQuery request, CancellationToken cancellationToken)
            {
                GetTrangloRoleByRoleCodeOutputDTO result = new GetTrangloRoleByRoleCodeOutputDTO();
                List<ScreenAccessMenu> screenAccessMenuList = new List<ScreenAccessMenu>();

                var _connectionString = _config.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                       "dbo.GetTrangloRoleByCode",
                       new
                       {
                           RoleCode = request.RoleCode
                       },
                       null, null, CommandType.StoredProcedure);
                    result = reader.ReadFirstOrDefault<GetTrangloRoleByRoleCodeOutputDTO>();
                }

                ScreenAccessHelper screenAccessHelper = new ScreenAccessHelper(_config, _accessControlManager);

                screenAccessMenuList = await screenAccessHelper.GetScreenAccessList(PortalCode.Admin, request.RoleCode);

                result.ScreenAccessMenuList = screenAccessMenuList;

                return Result.Success<GetTrangloRoleByRoleCodeOutputDTO>(result);
            }               
        }
    }
}
