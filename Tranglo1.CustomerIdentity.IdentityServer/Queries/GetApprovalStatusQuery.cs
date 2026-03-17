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
    public class GetApprovalStatusQuery : IRequest<IEnumerable<ApprovalStatusOutputDTO>>
    {
        public class GetApprovalStatusQueryHandler : IRequestHandler<GetApprovalStatusQuery, IEnumerable<ApprovalStatusOutputDTO>>
        {
            private readonly BusinessProfileDbContext _context;
            private readonly IMapper _mapper;

            public GetApprovalStatusQueryHandler(BusinessProfileDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ApprovalStatusOutputDTO>> Handle(GetApprovalStatusQuery query, CancellationToken cancellationToken)
            {
                return await _context.ApprovalStatuses.ProjectTo<ApprovalStatusOutputDTO>(_mapper.ConfigurationProvider)
                     .ToListAsync(cancellationToken);
            }
        }
    }
}
