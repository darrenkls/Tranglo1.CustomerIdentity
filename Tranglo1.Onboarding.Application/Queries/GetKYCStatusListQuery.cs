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

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
	public class GetKYCStatusListQuery : IRequest<IEnumerable<KYCStatusListOutputDTO>>
	{
        public class GetKYCStatusListQueryHandler : IRequestHandler<GetKYCStatusListQuery, IEnumerable<KYCStatusListOutputDTO>>
        {
            private readonly IBusinessProfileRepository _repository;
            private readonly IMapper _mapper;
            public GetKYCStatusListQueryHandler(IBusinessProfileRepository repo, IMapper mapper)
            {
                _repository = repo;
                _mapper = mapper;
            }

            public async Task<IEnumerable<KYCStatusListOutputDTO>> Handle(GetKYCStatusListQuery query, CancellationToken cancellationToken)
            {
                Specification<KYCStatus> spec = Specification<KYCStatus>.All;
                return _mapper.Map<IEnumerable<KYCStatus>, IEnumerable<KYCStatusListOutputDTO>>(await _repository.GetKYCStatusesAsync(spec));
            
            }
        }
    }
}
