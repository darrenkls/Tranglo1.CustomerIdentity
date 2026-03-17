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
    public class GetSolutionsQuery : IRequest<IEnumerable<SolutionListOutputDTO>>
    {
        public long? CustomerTypeCode { get; set; }

        public class GetSolutionsQueryHandler : IRequestHandler<GetSolutionsQuery, IEnumerable<SolutionListOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetSolutionsQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper; 
            }

            public async Task<IEnumerable<SolutionListOutputDTO>> Handle(GetSolutionsQuery query, CancellationToken cancellationToken)
            {
                var solutionList = await _context.Solutions.ProjectTo<SolutionListOutputDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                if (query.CustomerTypeCode != null)
                {
                    // If not Remittance Partner (MSB), hide Solution -> Connect
                    if (query.CustomerTypeCode != CustomerType.Remittance_Partner.Id)
                    {
                        solutionList.Remove(solutionList.Find(x => x.SolutionCode == Solution.Connect.Id));
                    }
                }                

                // Hide unused solutions from dropdown
                solutionList.Remove(solutionList.Find(x => x.SolutionCode == Solution.Personal.Id));
                solutionList.Remove(solutionList.Find(x => x.SolutionCode == Solution.Recharge.Id));
                solutionList.Remove(solutionList.Find(x => x.SolutionCode == Solution.Undefined.Id));

                return solutionList;
            }
        }
    }
}