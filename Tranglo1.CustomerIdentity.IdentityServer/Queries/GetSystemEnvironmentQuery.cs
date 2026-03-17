using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetSystemEnvironmentQuery : IRequest<IEnumerable<SystemEnvironmentListOutputDTO>>
    {
        public class GetSystemEnvironmentQueryHandler : IRequestHandler<GetSystemEnvironmentQuery, IEnumerable<SystemEnvironmentListOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetSystemEnvironmentQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<SystemEnvironmentListOutputDTO>> Handle(GetSystemEnvironmentQuery query, CancellationToken cancellationToken)
            {
                return await _context.SystemEnvironments.ProjectTo<SystemEnvironmentListOutputDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
