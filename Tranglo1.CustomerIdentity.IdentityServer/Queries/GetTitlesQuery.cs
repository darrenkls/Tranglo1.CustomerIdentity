using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Meta;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetTitlesQuery : IRequest<IEnumerable<TitleOutputDTO>>
    {
        public class GetTitlesQueryHandler : IRequestHandler<GetTitlesQuery, IEnumerable<TitleOutputDTO>>
        {
            private readonly BusinessProfileDbContext _context;
            private readonly IMapper _mapper;
            public GetTitlesQueryHandler(BusinessProfileDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<TitleOutputDTO>> Handle(GetTitlesQuery query, CancellationToken cancellationToken)
            {
                return await _context.Titles.ProjectTo<TitleOutputDTO>(_mapper.ConfigurationProvider)
                      .ToListAsync(cancellationToken);
            }
        }
    }
}
