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
    internal class GetTrangloSalesOperationStaffQuery : BaseQuery<Result<IEnumerable<KYCSalesOperationStaffOutputDTO>>>
    {
        public string LoginID { get; set; }
        public string Name { get; set; }
        public class GetTrangloSalesOperationStaffQueryHandler : IRequestHandler<GetTrangloSalesOperationStaffQuery, Result<IEnumerable<KYCSalesOperationStaffOutputDTO>>>
        {
            private readonly IConfiguration _config;

            public GetTrangloSalesOperationStaffQueryHandler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<IEnumerable<KYCSalesOperationStaffOutputDTO>>> Handle(GetTrangloSalesOperationStaffQuery request, CancellationToken cancellationToken)
            {
                var _connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                        "GetKYCSalesOperationStaff",
                        new
                        {

                        },
                        null, null, CommandType.StoredProcedure);
                    var result = await reader.ReadAsync<KYCSalesOperationStaffOutputDTO>();

                    return Result.Success<IEnumerable<KYCSalesOperationStaffOutputDTO>>(result);
                }
            }
        }
    }
}