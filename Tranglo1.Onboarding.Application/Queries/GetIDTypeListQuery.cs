using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
	public class GetIDTypeListQuery : IRequest<IEnumerable<IDTypeListOutputDTO>>
	{
        public class GetIDTypeListQueryHandler : IRequestHandler<GetIDTypeListQuery, IEnumerable<IDTypeListOutputDTO>>
        {
            private readonly IBusinessProfileRepository _repository;
            private readonly BusinessProfileDbContext _context;
            private readonly IMapper _mapper;
            public GetIDTypeListQueryHandler(BusinessProfileDbContext context, IMapper mapper, IBusinessProfileRepository repository)
            {
                _repository = repository;
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<IDTypeListOutputDTO>> Handle(GetIDTypeListQuery query, CancellationToken cancellationToken)
            {
                Specification<IDType> spec = Specification<IDType>.All;

                var idTypes = await _repository.GetIDTypesAsync(spec);
                return _mapper.Map<IEnumerable<IDType>, IEnumerable<IDTypeListOutputDTO>>(idTypes);

            }
        }
    }
}
