using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetExternalUserRoleStatusQuery : BaseQuery<IEnumerable<ExternalUserRoleStatusOutputDTO>>
    {
    }

    internal class GetExternalUserRoleStatusQueryHandler : IRequestHandler<GetExternalUserRoleStatusQuery, IEnumerable<ExternalUserRoleStatusOutputDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ApplicationUserDbContext _context;


        public GetExternalUserRoleStatusQueryHandler(IMapper mapper, ApplicationUserDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<ExternalUserRoleStatusOutputDTO>> Handle(GetExternalUserRoleStatusQuery request, CancellationToken cancellationToken)
        {
            return await _context.ExternalUserRoleStatuses.ProjectTo<ExternalUserRoleStatusOutputDTO>(_mapper.ConfigurationProvider)
                                  .ToListAsync(cancellationToken);
        }
    }
}
