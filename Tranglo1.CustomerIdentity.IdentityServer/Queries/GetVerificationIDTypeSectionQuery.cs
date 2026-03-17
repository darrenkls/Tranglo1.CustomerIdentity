using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Meta;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetVerificationIDTypeSectionQuery : IRequest<IEnumerable<VerificationIDTypeSectionOutputDTO>>
    {
        public class GetVerificationIDTypeSectionQueryHandler : IRequestHandler<GetVerificationIDTypeSectionQuery, IEnumerable<VerificationIDTypeSectionOutputDTO>>
        {
            private readonly IBusinessProfileRepository _repository;

            public GetVerificationIDTypeSectionQueryHandler(IBusinessProfileRepository repository)
            {
                _repository = repository;
            }


            public async Task<IEnumerable<VerificationIDTypeSectionOutputDTO>> Handle(GetVerificationIDTypeSectionQuery request, CancellationToken cancellationToken)
            {
                var verificationIDTypeSections = await _repository.GetAllVerificationIDTypeSections();

                IEnumerable<VerificationIDTypeSectionOutputDTO> outputDTO = verificationIDTypeSections.Select(a => new VerificationIDTypeSectionOutputDTO
                {
                    VerificationIDTypeSectionCode = a?.Id,
                    VerificationIDTypeCode = a.VerificationIDType?.Id,
                    VerificationIDTypeDescription = a.Description
                });

                return outputDTO;
            }
        }
    }
}
