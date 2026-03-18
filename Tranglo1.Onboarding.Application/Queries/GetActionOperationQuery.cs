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
    public class GetActionOperationQuery : IRequest<IEnumerable<ActionOperationOutputDTO>>
    {
        public class GetActionOperationQueryandler : IRequestHandler<GetActionOperationQuery, IEnumerable<ActionOperationOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetActionOperationQueryandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ActionOperationOutputDTO>> Handle(GetActionOperationQuery query, CancellationToken cancellationToken)
            {
                return await _context.ActionOperations.ProjectTo<ActionOperationOutputDTO>(_mapper.ConfigurationProvider)
                      .ToListAsync(cancellationToken);
            }
        }
    }
}
