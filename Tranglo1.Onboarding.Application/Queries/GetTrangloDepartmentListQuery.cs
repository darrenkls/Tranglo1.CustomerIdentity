using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetTrangloDepartmentListQuery : IRequest<IEnumerable<TrangloDepartmentListOutputDTO>>
    {
        public class GetTrangloDepartmentListQueryHandler : IRequestHandler<GetTrangloDepartmentListQuery, IEnumerable<TrangloDepartmentListOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetTrangloDepartmentListQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<TrangloDepartmentListOutputDTO>> Handle(GetTrangloDepartmentListQuery query, CancellationToken cancellationToken)
            {
                return await _context.TrangloDepartment.ProjectTo<TrangloDepartmentListOutputDTO>(_mapper.ConfigurationProvider).OrderBy(x => x.Description)
                      .ToListAsync(cancellationToken);
            }
        }
    }
}
