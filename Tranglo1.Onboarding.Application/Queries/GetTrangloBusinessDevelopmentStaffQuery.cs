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
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;



namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetTrangloBusinessDevelopmentStaffQuery : BaseQuery<Result<IEnumerable<KYCBusinessDevelopmentStaffOutputDTO>>>
    {
        public string LoginID { get; set; }
        public string Name { get; set; }
        public class GetTrangloBusinessDevelopmentStaffQueryHandler : IRequestHandler<GetTrangloBusinessDevelopmentStaffQuery, Result<IEnumerable<KYCBusinessDevelopmentStaffOutputDTO>>>
        {
            private readonly IConfiguration _config;

            public GetTrangloBusinessDevelopmentStaffQueryHandler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<IEnumerable<KYCBusinessDevelopmentStaffOutputDTO>>> Handle(GetTrangloBusinessDevelopmentStaffQuery request, CancellationToken cancellationToken)
            {
                var _connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                        "GetKYCBusinessDevelopmentStaff",
                        new
                        {

                        },
                        null, null, CommandType.StoredProcedure);
                    var result = await reader.ReadAsync<KYCBusinessDevelopmentStaffOutputDTO>();
                    var sortedResult = result.OrderBy(x => x.BusinessDevelopmentStaffAssignedName);
                    return Result.Success<IEnumerable<KYCBusinessDevelopmentStaffOutputDTO>>(sortedResult);
                }
            }
        }
    }
}
