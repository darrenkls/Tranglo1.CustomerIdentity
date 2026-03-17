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
	public class GetReviewResultListQuery : IRequest<IEnumerable<ReviewResultListOutputDTO>>
	{
        public class GetReviewResultListQueryHandler : IRequestHandler<GetReviewResultListQuery, IEnumerable<ReviewResultListOutputDTO>>
        {
            private readonly IBusinessProfileRepository _repository;
            private readonly IMapper _mapper;
            public GetReviewResultListQueryHandler(IBusinessProfileRepository repo, IMapper mapper)
            {
                _repository = repo;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ReviewResultListOutputDTO>> Handle(GetReviewResultListQuery query, CancellationToken cancellationToken)
            {
                Specification<ReviewResult> spec = Specification<ReviewResult>.All;
                return _mapper.Map<IEnumerable<ReviewResult>, IEnumerable<ReviewResultListOutputDTO>>(await _repository.GetReviewResultsAsync(spec));
            
            }
        }
    }
}
