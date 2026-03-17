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
    public class GetRoleStatusQuery : IRequest<IEnumerable<RoleStatusOutputDTO>>
    {
        public class GetRoleStatusQueryHandler : IRequestHandler<GetRoleStatusQuery, IEnumerable<RoleStatusOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetRoleStatusQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<RoleStatusOutputDTO>> Handle(GetRoleStatusQuery query, CancellationToken cancellationToken)
            {
                return await _context.RoleStatus.ProjectTo<RoleStatusOutputDTO>(_mapper.ConfigurationProvider)
                      .ToListAsync(cancellationToken);
            }
        }

    }
}
