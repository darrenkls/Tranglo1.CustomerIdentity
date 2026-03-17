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
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Watchlist;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetScreeningCountersQuery : IRequest<ScreeningCounterOutputDTO>
    {
        public int BusinessProfileCode { get; set; }
        public int OwnershipStructureType { get; set; }
        public int TableId { get; set; }
        public class GetScreeningCountersQueryHandler : IRequestHandler<GetScreeningCountersQuery, ScreeningCounterOutputDTO>
        {
            private readonly IConfiguration _config;

            public GetScreeningCountersQueryHandler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<ScreeningCounterOutputDTO> Handle(GetScreeningCountersQuery request, CancellationToken cancellationToken)
            {
                ScreeningCounterOutputDTO result;
                var _connectionString = _config.GetConnectionString("DefaultConnection");


                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                       "GetScreeningCounter",
                       new
                       {
                           BusinessProfileCode = request.BusinessProfileCode,
                           OwnershipStructureTypeId = request.OwnershipStructureType,
                           TableId = request.TableId
                       },
                       null, null, CommandType.StoredProcedure);

                    
                    result = await reader.ReadFirstOrDefaultAsync<ScreeningCounterOutputDTO>();
                    return result;
                }
            }
        }
    }
}
