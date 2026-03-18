using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Meta;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetPartnerRegistrationLeadsOriginsQuery : IRequest<IEnumerable<PartnerRegistrationLeadsOriginOutputDTO>>
    {
        public class GetPartnerRegistrationLeadsOriginsQueryHandler : IRequestHandler<GetPartnerRegistrationLeadsOriginsQuery, IEnumerable<PartnerRegistrationLeadsOriginOutputDTO>>
        {
            public Task<IEnumerable<PartnerRegistrationLeadsOriginOutputDTO>> Handle(GetPartnerRegistrationLeadsOriginsQuery request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Enumeration.GetAll<PartnerRegistrationLeadsOrigin>()
                    .Select(x => new PartnerRegistrationLeadsOriginOutputDTO
                    {
                        PartnerRegistrationLeadsOriginCode = x.Id,
                        Description = x.Name
                    })
                    .OrderBy(x => x.PartnerRegistrationLeadsOriginCode)
                    .AsEnumerable());
            }
        }
    }
}
