using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Meta;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetCollectionTierQuery : IRequest<IReadOnlyList<CollectionTierOutputDTO>>
    {
        public class GetCollectionTierQueryHandler : IRequestHandler<GetCollectionTierQuery, IReadOnlyList<CollectionTierOutputDTO>>
        {
            private readonly BusinessProfileDbContext _businessProfileDbContext;
            private readonly IMapper _mapper;
            public GetCollectionTierQueryHandler(BusinessProfileDbContext businessProfileDbContext, IMapper mapper)
            {
                _businessProfileDbContext = businessProfileDbContext;
                _mapper = mapper;

            }
            public async Task<IReadOnlyList<CollectionTierOutputDTO>> Handle(GetCollectionTierQuery query, CancellationToken cancellationToken)
            {
                return await _businessProfileDbContext.CollectionTiers.ProjectTo<CollectionTierOutputDTO>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);
            }
        }
    }
}
