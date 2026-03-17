using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetSignUpCodeAccountStatusListQuery : IRequest<IEnumerable<SignUpCodeAccountStatusOutputDTO>>
    {
        public class GetSignUpCodeAccountStatusListQueryHandler : IRequestHandler<GetSignUpCodeAccountStatusListQuery, IEnumerable<SignUpCodeAccountStatusOutputDTO>>
        {
            private readonly ISignUpCodeRepository _repository;
            private readonly IMapper _mapper;
            public GetSignUpCodeAccountStatusListQueryHandler(ISignUpCodeRepository repo, IMapper mapper)
            {
                _repository = repo;
                _mapper = mapper;
            }

            public async Task<IEnumerable<SignUpCodeAccountStatusOutputDTO>> Handle(GetSignUpCodeAccountStatusListQuery request, CancellationToken cancellationToken)
            {
                return _mapper.Map<IEnumerable<SignUpAccountStatus>, IEnumerable<SignUpCodeAccountStatusOutputDTO>>(await _repository.GetSignUpCodeAccountStatusAsync());
            }
        }
    }
}