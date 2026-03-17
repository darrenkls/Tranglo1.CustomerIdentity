using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetFilteredSolutionsQuery : IRequest<IEnumerable<SolutionListOutputDTO>>
    {
        public class GetFilteredSolutionsQueryHandler : IRequestHandler<GetFilteredSolutionsQuery, IEnumerable<SolutionListOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetFilteredSolutionsQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<SolutionListOutputDTO>> Handle(GetFilteredSolutionsQuery query, CancellationToken cancellationToken)
            {
                var solutionList = await _context.Solutions.ProjectTo<SolutionListOutputDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return solutionList.FindAll(a => a.SolutionCode != Solution.Undefined.Id
                    && (a.SolutionCode == Solution.Connect.Id || a.SolutionCode == Solution.Business.Id)
                    );
            }
        }
    }
}
