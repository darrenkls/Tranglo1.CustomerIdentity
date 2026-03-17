using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.IdentityServer.Attributes;
using Tranglo1.CustomerIdentity.IdentityServer.Command;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.CountrySetting;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Security;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/country-settings")]
    [LogInputDTO]
    [LogOutputDTO]
    public class CountrySettingController : Controller
    {
        private readonly ILogger<CountrySettingController> _logger;
        public IMediator Mediator { get; }
        private readonly IMapper _mapper;

        public CountrySettingController(ILogger<CountrySettingController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            Mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieve list of Country Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = nameof(GetCountrySettingList), Tags = new[] { "Country Setting" })]
        public async Task<PagedResult<GetCountrySettingOutputDTO>> GetCountrySettingList([FromQuery] GetCountrySettingInputDTO countrySettingInputDTO)
        {
            GetCountrySettingsQuery query = new GetCountrySettingsQuery
            {
                InputDTO = countrySettingInputDTO
            };

            return await Mediator.Send(query);
        }

        /// <summary>
        /// Save New Country Setting
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(CountrySettingOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(SaveCountrySetting), Tags = new[] { "Country Setting" })]
        public async Task<IActionResult> SaveCountrySetting([FromBody] PostCountrySettingInputDTO countrySettingInputDTO)
        {
            AddCountrySettingCommand command = new AddCountrySettingCommand()
            {
                DTO = countrySettingInputDTO,
                UserBearerToken = Request.Headers["Authorization"].ToString()

            };

            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[SaveCountrySetting] {result.Error}");
                return ValidationProblem(result.Error);
            }
            return Ok(result.Value);

        }

        /// <summary>
        /// Update Country Setting 
        /// </summary>
        /// <param name="countrySettingCode"></param>
        /// <param name="countrySettingInputDTO"></param>
        /// <returns></returns>
        [HttpPut("{countrySettingCode}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(CountrySettingOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(UpdateCountrySetting), Tags = new[] { "Country Setting" })]
        public async Task<IActionResult> UpdateCountrySetting(int countrySettingCode, [FromBody] PutCountrySettingInputDTO countrySettingInputDTO)
        {
            UpdateCountrySettingCommand command = new UpdateCountrySettingCommand()
            {
                CountrySettingCode = countrySettingCode,
                DTO = countrySettingInputDTO,
                UserBearerToken = Request.Headers["Authorization"].ToString()
            };

            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[UpdateCountrySetting] countrySettingCode:{countrySettingCode}");
                _logger.LogError($"[UpdateCountrySetting] {result.Error}");
                return ValidationProblem(result.Error);
            }
            return Ok(result.Value);

        }

        /// <summary>
        /// Delete Country Setting
        /// </summary>
        /// <param name="countrySettingCode"></param>
        /// <returns></returns>
        [HttpDelete("{countrySettingCode}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(CountrySettingOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(DeleteCountrySetting), Tags = new[] { "Country Setting" })]
        public async Task<IActionResult> DeleteCountrySetting(int countrySettingCode)
        {
            DeleteCountrySettingCommand command = new DeleteCountrySettingCommand()
            {
                CountrySettingCode = countrySettingCode,
                UserBearerToken = Request.Headers["Authorization"].ToString()
            };

            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[DeleteCountrySetting] {result.Error}");
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }
    }
}
