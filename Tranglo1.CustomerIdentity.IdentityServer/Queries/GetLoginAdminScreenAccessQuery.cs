using AutoMapper;
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
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetLoginAdminScreenAccessQuery : BaseQuery<Result<IEnumerable<UACLoginSubMenuActionOutputDTO>>>
    {
        public string RoleCode { get; set; }

        public override Task<string> GetAuditLogAsync(Result<IEnumerable<UACLoginSubMenuActionOutputDTO>> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Get Admin Login Screen Access List based on role";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);

        }
    }

    internal class GetLoginAdminScreenAccessQueryHandler : IRequestHandler<GetLoginAdminScreenAccessQuery, Result<IEnumerable<UACLoginSubMenuActionOutputDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly AccessControlManager _accessControlManager;
        public GetLoginAdminScreenAccessQueryHandler(IMapper mapper, IConfiguration config, AccessControlManager accessControlManager)
        {
            _mapper = mapper;
            _config = config;
            _accessControlManager = accessControlManager;
        }

        public async Task<Result<IEnumerable<UACLoginSubMenuActionOutputDTO>>> Handle(GetLoginAdminScreenAccessQuery request, CancellationToken cancellationToken)
        {
            var _connectionString = _config.GetConnectionString("DefaultConnection");

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("role",request.RoleCode);

            var claims = _accessControlManager.GetClaimListing(keyValuePairs);

            IEnumerable<UACLoginSubMenuActionOutputDTO> screenAccessOutputDTOs;
            IEnumerable<UACLoginSubMenuPermissionOutputDTO> actionOutputDTOs;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var reader = await connection.QueryMultipleAsync(
                    "GetLoginScreenAccess",
                    new
                    {
                        Claims = claims
                    },
                    null, null, CommandType.StoredProcedure);

                // read as IEnumerable<dynamic>
                screenAccessOutputDTOs = await reader.ReadAsync<UACLoginSubMenuActionOutputDTO>();
                actionOutputDTOs = await reader.ReadAsync<UACLoginSubMenuPermissionOutputDTO>();

            }

            foreach (UACLoginSubMenuActionOutputDTO screenAccessOutputDTO in screenAccessOutputDTOs)
            {
                screenAccessOutputDTO.UACActions = actionOutputDTOs.Where(x => x.SubMenuCode == screenAccessOutputDTO.SubMenuCode).ToList();
            }

            return Result.Success<IEnumerable<UACLoginSubMenuActionOutputDTO>>(screenAccessOutputDTOs);

        }
    }
}

