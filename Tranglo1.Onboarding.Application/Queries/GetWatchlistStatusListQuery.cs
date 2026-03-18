using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetWatchlistStatusListQuery : IRequest<IEnumerable<WatchlistStatusListOutputDTO>>
    {
        public class GetWatchlistStatusListQueryHandler : IRequestHandler<GetWatchlistStatusListQuery, IEnumerable<WatchlistStatusListOutputDTO>>
        {
            private readonly ScreeningDBContext _context;
            private readonly IMapper _mapper;
            public GetWatchlistStatusListQueryHandler(ScreeningDBContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<WatchlistStatusListOutputDTO>> Handle(GetWatchlistStatusListQuery query, CancellationToken cancellationToken)
            {
                return await _context.WatchlistStatuses.ProjectTo<WatchlistStatusListOutputDTO>(_mapper.ConfigurationProvider)
                      .ToListAsync(cancellationToken);
            }
        }
    }
}
