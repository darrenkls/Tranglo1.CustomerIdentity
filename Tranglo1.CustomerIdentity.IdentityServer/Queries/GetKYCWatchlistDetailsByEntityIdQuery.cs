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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities.ScreeningAggregate;
using Tranglo1.CustomerIdentity.Domain.ExternalServices.Compliance;
using Tranglo1.CustomerIdentity.Domain.ExternalServices.Compliance.Models.Responses;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetKYCWatchlistDetailsByEntityIdQuery: BaseQuery<Result<GetEntityDetailByReferenceCodeAndEntityIdResponse>>
    {
        
        public Guid ReferenceCode { get; set; }
        public long EntityId { get; set; }
        public int ListSource { get; set; }

        public class GetKYCWatchlistDetailsByEntityIdQueryHandler : IRequestHandler<GetKYCWatchlistDetailsByEntityIdQuery, Result<GetEntityDetailByReferenceCodeAndEntityIdResponse>>
        {
            private readonly IComplianceExternalService _complianceExternalService;

            public GetKYCWatchlistDetailsByEntityIdQueryHandler(IComplianceExternalService complianceExternalService)
            {
                _complianceExternalService = complianceExternalService;
            }

            public async Task<Result<GetEntityDetailByReferenceCodeAndEntityIdResponse>> Handle(GetKYCWatchlistDetailsByEntityIdQuery request, CancellationToken cancellationToken)
            {
                var listSource = Enumeration.FindById<ScreeningListSource>(request.ListSource);
                if (listSource == null)
                {
                    return Result.Failure<GetEntityDetailByReferenceCodeAndEntityIdResponse>($"Invalid List Source: {request.ListSource}");
                }

                var result = await _complianceExternalService.GetEntityDetailAsync(request.ReferenceCode, request.EntityId, listSource);

                if (result.IsFailure)
                {
                    return Result.Failure<GetEntityDetailByReferenceCodeAndEntityIdResponse>(result.Error);
                    
                }

                return Result.Success(result.Value);
            }
        }
    }
}