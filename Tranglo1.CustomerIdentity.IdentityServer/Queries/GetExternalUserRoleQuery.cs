using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.Helper.ACL;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.PartnerManageExternalRole, UACAction.View)] 
    internal class GetExternalUserRoleQuery : BaseQuery<Result<ExternalUserRoleOutputDTO>>
    {
        public string RoleCode { get; set; }
        public int SolutionCode { get; set; }

        public override Task<string> GetAuditLogAsync(Result<ExternalUserRoleOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Get External User Role";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class GetExternalUserRoleQueryHandler : IRequestHandler<GetExternalUserRoleQuery, Result<ExternalUserRoleOutputDTO>>
    {
        private readonly IConfiguration _config;
        private readonly AccessControlManager _accessControlManager;

        public GetExternalUserRoleQueryHandler(IConfiguration config, AccessControlManager accessControlManager)
        {
            _config = config;
            _accessControlManager = accessControlManager;
        }

        public async Task<Result<ExternalUserRoleOutputDTO>> Handle(GetExternalUserRoleQuery request, CancellationToken cancellationToken)
        {
            ExternalUserRoleOutputDTO result = new ExternalUserRoleOutputDTO();
            
            var _connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var reader = await connection.QueryMultipleAsync(
                   "GetExternalUserRoleByCode",
                   new
                   {
                       RoleCode = request.RoleCode,
                   },
                   null, null, CommandType.StoredProcedure);

                result = await reader.ReadFirstOrDefaultAsync<ExternalUserRoleOutputDTO>();
            }

            var screenAccessHelper = new ScreenAccessHelper(_config, _accessControlManager);
            result.ScreenAccessMenuList = await screenAccessHelper.GetScreenAccessList((PortalCode)request.SolutionCode, request.RoleCode);

            return Result.Success(result);
        }
    }
}
