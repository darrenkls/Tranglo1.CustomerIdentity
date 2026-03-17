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
    public class GetUserTypeQuery : IRequest<IEnumerable<UserTypeListOutputDTO>>
    {
        public class GetUserTypeQueryHandler : IRequestHandler<GetUserTypeQuery, IEnumerable<UserTypeListOutputDTO>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;
            public GetUserTypeQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper; 
            }
            public async Task<IEnumerable<UserTypeListOutputDTO>> Handle(GetUserTypeQuery query, CancellationToken cancellationToken)
            {

                var userTypeList = await _context.UserTypes.ProjectTo<UserTypeListOutputDTO>(_mapper.ConfigurationProvider)                    
                    .ToListAsync(cancellationToken);

                return userTypeList.FindAll(a => a.UserTypeCode != UserType.Undefined.Id);
            }
        }
    }
}
