using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    ////[Permission("APIGetRolesQuery", "Get Role List", "RoleListing")]
    public class GetRolesByBusinessProfileQuery : IRequest<Result<IReadOnlyList<RoleListDto>>>
    {
        public int BusinessProfileCode { get; set; }

        public class GetRolesQueryHandler : IRequestHandler<GetRolesByBusinessProfileQuery, Result<IReadOnlyList<RoleListDto>>>
        {
            private readonly IConfiguration _config;

            public GetRolesQueryHandler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<Result<IReadOnlyList<RoleListDto>>> Handle(GetRolesByBusinessProfileQuery query, CancellationToken cancellationToken)
            {
                List<RoleListDto> rolesAvailableToInvitee = new List<RoleListDto>();
                string _connectionString = _config.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"SELECT R.[RoleCode] AS RoleCode,
                                               R.[RoleName] AS Description
                                        FROM [Tranglo1_CustomerIdentity].[dbo].[Roles] AS R
                                        INNER JOIN [Tranglo1_CustomerIdentity].[dbo].[Departments] AS D ON R.DepartmentCode = D.DepartmentCode
                                        INNER JOIN [Tranglo1_CustomerIdentity].[dbo].[PartnerCompanies] AS PC ON D.CompanyCode = PC.CompanyCode
                                        WHERE PC.BusinessProfileCode=@BusinessProfileCode";

                    IEnumerable<RoleListDto> resultList = await connection.QueryAsync<RoleListDto>(
                                                                            command: new CommandDefinition(
                                                                                sqlQuery,
                                                                                parameters: new
                                                                                {
                                                                                    BusinessProfileCode = query.BusinessProfileCode
                                                                                },
                                                                                cancellationToken: cancellationToken
                                                                                )
                                                                            );

                    rolesAvailableToInvitee = resultList.ToList();
                }

                return Result.Success<IReadOnlyList<RoleListDto>>(rolesAvailableToInvitee);
            }
        }
    }
}
