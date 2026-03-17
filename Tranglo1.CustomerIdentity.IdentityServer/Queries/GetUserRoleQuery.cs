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
    public class GetUserRoleQuery : IRequest<IEnumerable<UserRolesOutputDTO>>
    {
        public class GetUserRoleQueryHandler : IRequestHandler<GetUserRoleQuery, IEnumerable<UserRolesOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetUserRoleQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<UserRolesOutputDTO>> Handle(GetUserRoleQuery query, CancellationToken cancellationToken)
            {
                return await _context.UserRoles.ProjectTo<UserRolesOutputDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
