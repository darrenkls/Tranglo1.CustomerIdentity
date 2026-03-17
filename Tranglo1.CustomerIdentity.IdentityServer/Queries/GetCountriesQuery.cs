using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetCountriesQuery : IRequest<IReadOnlyList<CountryListOutputDTO>>
    {
        public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, IReadOnlyList<CountryListOutputDTO>>
        {
            private readonly ICountrySettingRepository _repository;
            //private readonly ICountryRepository _countryRepository;
            private readonly IMapper _mapper;
            public GetCountriesQueryHandler(
                IMapper mapper, 
                ICountrySettingRepository repository)
            {
                //_countryRepository = countryRepository;
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<IReadOnlyList<CountryListOutputDTO>> Handle(GetCountriesQuery query, CancellationToken cancellationToken)
            {

                var countriesMeta = CountryMeta.GetAllCountryMeta();
                return _mapper.Map<IReadOnlyList<CountryListOutputDTO>>(countriesMeta);

                /*
                var specs = Specification<Country>.All;
                var IsNotSanction = new NotSanctionCountrySpecification();
                specs = specs.And(IsNotSanction);

                var list= await _countryRepository.GetCountriesAsync(specs);
                return _mapper.Map<IReadOnlyList<CountryListOutputDTO>>(list);
                */

            }
        }
    }
}
