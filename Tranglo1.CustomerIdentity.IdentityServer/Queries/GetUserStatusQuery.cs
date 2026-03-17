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
    public class GetUserStatusQuery : IRequest<IEnumerable<UserStatusListOutputDTO>>
    {
        public class GetUserStatusQueryHandler : IRequestHandler<GetUserStatusQuery, IEnumerable<UserStatusListOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetUserStatusQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper; 
            }

            public async Task<IEnumerable<UserStatusListOutputDTO>> Handle(GetUserStatusQuery query, CancellationToken cancellationToken)
            {
                return await _context.AccountStatus.ProjectTo<UserStatusListOutputDTO>(_mapper.ConfigurationProvider)
                      .ToListAsync(cancellationToken);
            }
        }
    }
}
