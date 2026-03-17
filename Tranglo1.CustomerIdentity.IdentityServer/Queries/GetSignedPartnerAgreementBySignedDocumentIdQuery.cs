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
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Partner;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{

    //[Permission(PermissionGroupCode.PartnerAgreement, UACAction.View)]
    internal class GetSignedPartnerAgreementBySignedDocumentIdQuery : BaseQuery<Result<SignedPartnerAgreementOutputDTO>>
    {
        public long PartnerCode { get; set; }
        public Guid SignedDocumentId { get; set; }

        public class GetSignedPartnerAgreementBySignedDocumentIdQueryHandler : IRequestHandler<GetSignedPartnerAgreementBySignedDocumentIdQuery, Result<SignedPartnerAgreementOutputDTO>>
        {
            private readonly IConfiguration _config;
            private readonly ILogger<GetSignedPartnerAgreementBySignedDocumentIdQueryHandler> _logger;

            public GetSignedPartnerAgreementBySignedDocumentIdQueryHandler(IConfiguration config, ILogger<GetSignedPartnerAgreementBySignedDocumentIdQueryHandler> logger)
            {
                _config = config;
                _logger = logger;
            }

            public async Task<Result<SignedPartnerAgreementOutputDTO>> Handle(GetSignedPartnerAgreementBySignedDocumentIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    SignedPartnerAgreementOutputDTO result;
                    var _connectionString = _config.GetConnectionString("DefaultConnection");

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        var reader = await connection.QueryMultipleAsync(
                           "GetSignedPartnerAgreement",
                           new
                           {
                               PartnerCode = request.PartnerCode,
                               SignedDocumentId = request.SignedDocumentId
                           },
                           null, null, CommandType.StoredProcedure); ;

                        result = reader.ReadFirstOrDefault<SignedPartnerAgreementOutputDTO>();
                        return Result.Success(result);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[GetSignedPartnerAgreementBySignedDocumentIdQuery] {ex.Message}");
                }
                return Result.Failure<SignedPartnerAgreementOutputDTO>(
                            $"Get signed partner agreement failed for {request.PartnerCode}."
                        );
            }
        }
    }
}
