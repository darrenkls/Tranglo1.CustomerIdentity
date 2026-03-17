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
    public class GetSignUpCodeLeadsOriginListQuery : IRequest<IEnumerable<SignUpCodesLeadsOriginOutputDTO>>
    {
        public class GetSignUpCodeLeadsOriginListQueryHandler : IRequestHandler<GetSignUpCodeLeadsOriginListQuery, IEnumerable<SignUpCodesLeadsOriginOutputDTO>>
        {
            private readonly ISignUpCodeRepository _repository;
            private readonly IMapper _mapper;
            public GetSignUpCodeLeadsOriginListQueryHandler(ISignUpCodeRepository repo, IMapper mapper)
            {
                _repository = repo;
                _mapper = mapper;
            }

            public async Task<IEnumerable<SignUpCodesLeadsOriginOutputDTO>> Handle(GetSignUpCodeLeadsOriginListQuery request, CancellationToken cancellationToken)
            {
                return _mapper.Map<IEnumerable<LeadsOrigin>, IEnumerable<SignUpCodesLeadsOriginOutputDTO>>(await _repository.GetSignUpCodeLeadsOriginAsync());
            }

          
        }
    }
}