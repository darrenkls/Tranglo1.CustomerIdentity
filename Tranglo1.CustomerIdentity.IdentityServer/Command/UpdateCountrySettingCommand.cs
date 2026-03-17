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
    //[Permission(PermissionGroupCode.KYCSanctionCountryManagement, UACAction.Edit)]
    [Permission(Permission.KYCAdministrationSanctionCountryManagement.Action_Edit_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.KYCAdministrationSanctionCountryManagement.Action_View_Code })]
    internal class UpdateCountrySettingCommand : BaseCommand<Result<CountrySettingOutputDTO>>
    {
        public int CountrySettingCode { get; set; }
        public PutCountrySettingInputDTO DTO { get; set; }
        public string UserBearerToken { get; set; }

        public override Task<string> GetAuditLogAsync(Result<CountrySettingOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Edited Country Risk Type";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class UpdateCountrySettingCommandHandler : IRequestHandler<UpdateCountrySettingCommand, Result<CountrySettingOutputDTO>>
    {
        private readonly ILogger<UpdateCountrySettingCommandHandler> _logger;
        private readonly ICountrySettingRepository _repository;
        private readonly IMapper _mapper;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        private IHttpClientFactory _httpClientFactory;
        private class ApiResponse
        {
            public string Detail { get; set; }
        }

        public UpdateCountrySettingCommandHandler(
            ILogger<UpdateCountrySettingCommandHandler> logger,
            ICountrySettingRepository repository,
            IMapper mapper,
            IConfiguration config,
            IHttpClientFactory httpClientFactory
            )
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Result<CountrySettingOutputDTO>> Handle(UpdateCountrySettingCommand request, CancellationToken cancellationToken)
        {
            var countrySetting = await _repository.GetCountrySettingByCodeAsync(request.CountrySettingCode);

            if (countrySetting == null)
            {
                _logger.LogError($"[UpdateCountrySetting] CountrySettingCode: {request.CountrySettingCode} not found.");
                return Result.Failure<CountrySettingOutputDTO>($"No Country Setting found for CountrySettingCode: {request.CountrySettingCode}");
            }

            //if (!request.DTO.IsHighRisk && !request.DTO.SanctionCount)
            //{
            //    return Result.Failure<CountrySettingOutputDTO>(
            //                $"At least one type(IsHighRisk/SanctionCount) must be True"
            //            );
            //}

            countrySetting.IsHighRisk = request.DTO.IsHighRisk;
            countrySetting.IsSanction = request.DTO.IsSanction;
            countrySetting.IsDisplay = request.DTO.IsDisplay;
            countrySetting.IsRejectTransaction = request.DTO.IsRejectTransaction;

            var result = await _repository.SaveCountrySettingAsync(countrySetting);

            if (result.IsFailure)
            {
                return Result.Failure<CountrySettingOutputDTO>(
                            $"Failed to update Country Setting for CountrySettingCode: {request.CountrySettingCode}"
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
