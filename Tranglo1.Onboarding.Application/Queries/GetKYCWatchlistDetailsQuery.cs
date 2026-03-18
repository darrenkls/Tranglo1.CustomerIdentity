using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.ExternalServices.Compliance;
using Tranglo1.CustomerIdentity.Domain.ExternalServices.Compliance.Models.Responses;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetKYCWatchlistDetailsQuery: BaseQuery<Result<List<GetEntityDetailByReferenceCodeResponse>>>
    {
        
        public Guid ReferenceCode { get; set; }

        public class GetKYCWatchlistDetailsQueryHandler : IRequestHandler<GetKYCWatchlistDetailsQuery, Result<List<GetEntityDetailByReferenceCodeResponse>>>
        {
            private readonly IComplianceExternalService _complianceExternalService;

            public GetKYCWatchlistDetailsQueryHandler(IComplianceExternalService complianceExternalService)
            {
                _complianceExternalService = complianceExternalService;
            }

            public async Task<Result<List<GetEntityDetailByReferenceCodeResponse>>> Handle(GetKYCWatchlistDetailsQuery request, CancellationToken cancellationToken)
            {
                var result = await _complianceExternalService.GetEntityDetailsByReferenceCodeAsync(request.ReferenceCode);

                if (result.IsFailure)
                {
                    return Result.Failure<List<GetEntityDetailByReferenceCodeResponse>>(result.Error);
                }

                return Result.Success(result.Value.ToList());
            }
        }
    }
}