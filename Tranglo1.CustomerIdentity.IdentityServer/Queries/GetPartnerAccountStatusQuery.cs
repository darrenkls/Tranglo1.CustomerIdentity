using AutoMapper;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.IdentityServer.Command;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Partner;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.PartnerAccountStatus, UACAction.View)]
    [Permission(Permission.ManagePartnerAccountStatus.Action_View_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { })]
    internal class GetPartnerAccountStatusQuery : BaseCommand<Result<IEnumerable<PartnerAccountStatusOutputDTO>>>
    {
        public long PartnerCode { get; set; }
        public long PartnerSubscriptionCode { get; set; }
    }
    public class GetPartnerAccountStatusQueryHandler : IRequestHandler<GetPartnerAccountStatusQuery, Result<IEnumerable<PartnerAccountStatusOutputDTO>>>
    {
        private readonly IConfiguration _config;

        public GetPartnerAccountStatusQueryHandler(IConfiguration config)
        {
            _config = config;
        }

        async Task<Result<IEnumerable<PartnerAccountStatusOutputDTO>>> IRequestHandler<GetPartnerAccountStatusQuery, Result<IEnumerable<PartnerAccountStatusOutputDTO>>>.Handle(GetPartnerAccountStatusQuery request, CancellationToken cancellationToken)
        {
            var _connectionString = _config.GetConnectionString("DefaultConnection");

            IEnumerable<PartnerAccountStatusOutputDTO> partnerAccountStatusOutputDTOs;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var reader = await connection.QueryMultipleAsync(
                    "GetPartnerAccountStatusQuery",
                    new
                    {
                        PartnerCode = request.PartnerCode,
                        PartnerSubscriptionCode = request.PartnerSubscriptionCode
                    },
                    null, null, CommandType.StoredProcedure);

                // read as IEnumerable<dynamic>
                partnerAccountStatusOutputDTOs = await reader.ReadAsync<PartnerAccountStatusOutputDTO>();
            }

            return Result.Success<IEnumerable<PartnerAccountStatusOutputDTO>>(partnerAccountStatusOutputDTOs);
        }
    }
}



