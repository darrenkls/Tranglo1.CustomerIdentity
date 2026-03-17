using AutoMapper;
using CSharpFunctionalExtensions;
using IdentityServer4.Extensions;
using System.Net.Http;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;

using System.Security.Claims;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.CountrySetting;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.UserAccessControl;
using Tranglo1.CustomerIdentity.Domain.Events;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.KYCSanctionCountryManagement, UACAction.Create)]
    [Permission(Permission.KYCAdministrationSanctionCountryManagement.Action_Add_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.KYCAdministrationSanctionCountryManagement.Action_View_Code })]
    internal class AddCountrySettingCommand : BaseCommand<Result<CountrySettingOutputDTO>>
    {
        public PostCountrySettingInputDTO DTO { get; set; }
        public string UserBearerToken { get; set; }

        public override Task<string> GetAuditLogAsync(Result<CountrySettingOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Add New High Risk / Sanctioned Country Tag";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class SaveCountrySettingCommandHandler : IRequestHandler<AddCountrySettingCommand, Result<CountrySettingOutputDTO>>
    {
        private readonly ICountrySettingRepository _repository;
        private readonly IMapper _mapper;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        private IHttpClientFactory _httpClientFactory;

        private class ApiResponse
        {
            public string Detail { get; set; }
        }

        public SaveCountrySettingCommandHandler(
            ICountrySettingRepository repository,
            IMapper mapper,
            IConfiguration config,
            IHttpClientFactory httpClientFactory
            )
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Result<CountrySettingOutputDTO>> Handle(AddCountrySettingCommand request, CancellationToken cancellationToken)
        {
            var country = CountryMeta.GetCountryByISO2Async(request.DTO.CountryISO2);

            if (country == null)
            {
                return Result.Failure<CountrySettingOutputDTO>(
                           $"CountryISO2: {request.DTO.CountryISO2} is not exists."
                       );
            }

            var existedCountrySetting = await _repository.IsCountrySettingExistAsync(request.DTO.CountryISO2);

            if (existedCountrySetting)
            {
                return Result.Failure<CountrySettingOutputDTO>(
                            $"This Country already exists."
                        );
            }

            //if (!request.DTO.IsHighRisk && !request.DTO.SanctionCount)
            //{
            //    return Result.Failure<CountrySettingOutputDTO>(
            //                $"At least one type(IsHighRisk/SanctionCount) must be True."
            //            );
            //}

            CountrySetting countrySetting = new CountrySetting()
            {
                Country = country,
                IsHighRisk = request.DTO.IsHighRisk,
                IsSanction = request.DTO.IsSanction,
                IsDisplay = request.DTO.IsDisplay,
                IsRejectTransaction = request.DTO.IsRejectTransaction
            };

            var result = await _repository.SaveCountrySettingAsync(countrySetting);

            if (result.IsFailure)
            {
                return Result.Failure<CountrySettingOutputDTO>(
                            $"Failed to save Country Setting for CountryISO2: [{request.DTO.CountryISO2}]"
                        );
            }

            var countrySettingsChangedEvent = new CountrySettingsChangedEvent
                (
                (int)countrySetting.Country.Id,
                countrySetting.Country.CountryISO2,
                countrySetting.IsHighRisk,
                countrySetting.IsSanction,
                countrySetting.IsDisplay,
                countrySetting.IsRejectTransaction
                );

            //Add to Event Table
            var countrySettingsChangedEventResult = await _repository.AddCountrySettingChangedEventAsync(countrySettingsChangedEvent);

            var output = new CountrySettingOutputDTO()
            {
                CountrySettingCode = result.Value.Id,
                CountryDescription = result.Value.Country.Name
            };

            return Result.Success(output);
        }
    }
}
