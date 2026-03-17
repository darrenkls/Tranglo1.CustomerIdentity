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
using Tranglo1.UserAccessControl;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.Domain.Events;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.KYCSanctionCountryManagement, UACAction.Edit)]
    [Permission(Permission.KYCAdministrationSanctionCountryManagement.Action_Remove_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.KYCAdministrationSanctionCountryManagement.Action_View_Code })]
    internal class DeleteCountrySettingCommand : BaseCommand<Result<CountrySettingOutputDTO>>
    {
        public int CountrySettingCode { get; set; }
        public string UserBearerToken { get; set; }

        public override Task<string> GetAuditLogAsync(Result<CountrySettingOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Removed Country From High Risk/Sanctioned Tag";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }    

    internal class DeleteCountrySettingCommandHandler : IRequestHandler<DeleteCountrySettingCommand, Result<CountrySettingOutputDTO>>
    {
        private readonly ICountrySettingRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCountrySettingCommandHandler> _logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        private IHttpClientFactory _httpClientFactory;
        private class ApiResponse
        {
            public string Detail { get; set; }
        }

        public DeleteCountrySettingCommandHandler(
            ICountrySettingRepository repository,
            IMapper mapper,
            ILogger<UpdateCountrySettingCommandHandler> logger,
            IConfiguration config,
            IHttpClientFactory httpClientFactory
            )
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Result<CountrySettingOutputDTO>> Handle(DeleteCountrySettingCommand request, CancellationToken cancellationToken)
        {
            var countrySetting = await _repository.GetCountrySettingByCodeAsync(request.CountrySettingCode);

            if (countrySetting == null)
            {
                _logger.LogError($"[DeleteCountrySetting] CountrySettingCode: {request.CountrySettingCode} not found.");
                return Result.Failure<CountrySettingOutputDTO>($"No Country Setting found for CountrySettingCode: {request.CountrySettingCode}");
            }

            var result = await _repository.DeleteCountrySettingAsync(countrySetting);

            if (result.IsFailure)
            {
                return Result.Failure<CountrySettingOutputDTO>(
                            $"Failed to delete Country Setting for CountryISO2: {countrySetting.Country.CountryISO2}"
                        );
            }

            //Set for CountrySettingsChangedEvent
            var isHighRisk = false;
            var isSanction = false;
            var isDisplay = true;
            var isRejectTransaction = false;

            var countrySettingsChangedEvent = new CountrySettingsChangedEvent
                (
                (int)countrySetting.Country.Id,
                countrySetting.Country.CountryISO2,
                isHighRisk,
                isSanction,
                isDisplay ,
                isRejectTransaction
                );

            //Add to Event table
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
