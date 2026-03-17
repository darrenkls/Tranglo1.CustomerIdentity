using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Meta;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetAuthorityLevelQuery : IRequest<IEnumerable<AuthorityLevelOutputDTO>>
    {
        public class GetAuthorityLevelQueryHandler : IRequestHandler<GetAuthorityLevelQuery, IEnumerable<AuthorityLevelOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetAuthorityLevelQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<AuthorityLevelOutputDTO>> Handle(GetAuthorityLevelQuery query, CancellationToken cancellationToken)
            {
                return await _context.AuthorityLevels.ProjectTo<AuthorityLevelOutputDTO>(_mapper.ConfigurationProvider)
                      .ToListAsync(cancellationToken);
            }
        }
    }
}
