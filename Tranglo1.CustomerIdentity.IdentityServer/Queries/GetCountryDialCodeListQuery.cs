using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetCountryDialCodeListQuery : IRequest<IReadOnlyList<CountryDialCodeOutputDTO>>
    {
        public class GetCountryDialCodeListQueryHandler : IRequestHandler<GetCountryDialCodeListQuery, IReadOnlyList<CountryDialCodeOutputDTO>>
        {
            private readonly IMapper _mapper;
            public GetCountryDialCodeListQueryHandler(IMapper mapper)
            {
                _mapper = mapper;
            }

            public async Task<IReadOnlyList<CountryDialCodeOutputDTO>> Handle(GetCountryDialCodeListQuery query, CancellationToken cancellationToken)
            {

                var list = CountryMeta.GetAllCountryMeta();
                return _mapper.Map<IReadOnlyList<CountryDialCodeOutputDTO>>(list);

            }
        }
    }
}
