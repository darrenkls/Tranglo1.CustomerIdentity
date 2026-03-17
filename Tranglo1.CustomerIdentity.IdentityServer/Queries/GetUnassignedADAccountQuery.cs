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

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetUnassignedADAccountQuery : IRequest<Result<IEnumerable<GetUnassignedADAccountOutputDTO>>>
    {
        
        public class GetSingleAdminQueryHandler : IRequestHandler<GetUnassignedADAccountQuery, Result<IEnumerable<GetUnassignedADAccountOutputDTO>>>
        {
            private readonly IConfiguration _config;

            public GetSingleAdminQueryHandler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<IEnumerable<GetUnassignedADAccountOutputDTO>>> Handle(GetUnassignedADAccountQuery request, CancellationToken cancellationToken)
            {
                IEnumerable<GetUnassignedADAccountOutputDTO> result;
                var _connectionString = _config.GetConnectionString("DefaultConnection");


                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                       "GetUnassignedAdAccountList",
                       new
                       {
                           
                       },
                       null, null, CommandType.StoredProcedure); ;


                    result = await reader.ReadAsync<GetUnassignedADAccountOutputDTO>();

                    return Result.Success<IEnumerable<GetUnassignedADAccountOutputDTO>>(result);
                }
            }
        }
    }
}
