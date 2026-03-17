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
    public class GetServicesOfferedQuery : IRequest<IEnumerable<ServicesOfferedOutputDTO>>
    {
        public class GetServicesOfferedQueryHandler : IRequestHandler<GetServicesOfferedQuery, IEnumerable<ServicesOfferedOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;

            public GetServicesOfferedQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ServicesOfferedOutputDTO>> Handle(GetServicesOfferedQuery query, CancellationToken cancellationToken)
            {
                return await _context.ServicesOffered.ProjectTo<ServicesOfferedOutputDTO>(_mapper.ConfigurationProvider)
                     .ToListAsync(cancellationToken);
            }
        }
    }
}
