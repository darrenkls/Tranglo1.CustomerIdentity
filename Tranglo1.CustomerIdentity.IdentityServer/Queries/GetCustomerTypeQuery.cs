using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Meta;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetCustomerTypeQuery : IRequest<IReadOnlyList<CustomerTypeOutputDTO>>
    {
        public class GetCustomerTypeQueryHandler : IRequestHandler<GetCustomerTypeQuery, IReadOnlyList<CustomerTypeOutputDTO>>
        {
            private readonly PartnerDBContext _partnerDBContext;
            private readonly IMapper _mapper;
            public GetCustomerTypeQueryHandler(PartnerDBContext partnerDBContext, IMapper mapper)
            {
                _partnerDBContext = partnerDBContext;
                _mapper = mapper;

            }
            public async Task<IReadOnlyList<CustomerTypeOutputDTO>> Handle(GetCustomerTypeQuery query, CancellationToken cancellationToken)
            {
                return await _partnerDBContext.CustomerTypes.ProjectTo<CustomerTypeOutputDTO>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);
            }
        }
    }
}
