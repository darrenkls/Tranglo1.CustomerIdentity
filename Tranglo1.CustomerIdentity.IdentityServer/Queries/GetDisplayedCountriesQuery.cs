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
    public class GetDisplayedCountriesQuery : IRequest<IReadOnlyList<CountryListOutputDTO>>
    {
        public class GetDisplayedCountriesQueryHandler : IRequestHandler<GetDisplayedCountriesQuery, IReadOnlyList<CountryListOutputDTO>>
        {
            private readonly ICountrySettingRepository _repository;
            //private readonly ICountryRepository _countryRepository;
            private readonly IMapper _mapper;
            public GetDisplayedCountriesQueryHandler(
                IMapper mapper, 
                ICountrySettingRepository repository)
            {
                //_countryRepository = countryRepository;
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<IReadOnlyList<CountryListOutputDTO>> Handle(GetDisplayedCountriesQuery query, CancellationToken cancellationToken)
            {
                var countriesMeta = await _repository.GetIsDisplayCountriesAsync();

                //Initialize the mapper
                var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<CountryMeta, CountryListOutputDTO>()
                    .ForMember(o => o.Description, act => act.MapFrom(m => m.Name))
                );

                //Using automapper
                var mapper = new Mapper(config);
                return mapper.Map<IReadOnlyList<CountryListOutputDTO>>(countriesMeta);

            }
        }
    }
}
