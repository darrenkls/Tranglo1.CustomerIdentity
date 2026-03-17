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
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Security;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers.SignUp
{
    [ApiController]
    [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/signup-codes")]
    [LogInputDTO]
    [LogOutputDTO]
    public class SignUpCodesController : Controller
    {
        private readonly ILogger<SignUpCodesController> _logger;
        public IMediator Mediator { get; }
        private readonly IMapper _mapper;

        public SignUpCodesController(ILogger<SignUpCodesController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            Mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieve list of SignUp codes
        /// </summary>
        /// /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortExpression"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = nameof(GetSignUpCodesList), Tags = new[] { "Sign Up Codes" })]
        public async Task<PagedResult<SignUpCodesOutputDTO>> GetSignUpCodesList(int pageSize, int pageIndex, string sortExpression, SortDirection sortDirection, string trangloEntity)
        {
            GetSignUpCodesQuery query = new GetSignUpCodesQuery
            { 
                PageSize = pageSize,
                PageIndex = pageIndex,
                SortExpression = sortExpression,
                Direction = sortDirection,
                TrangloEntity = trangloEntity
            };

            return await Mediator.Send(query);

        }

        /// <summary>
        /// Generate SignUp codes
        /// </summary>
        /// <param name="signUpCodesInputDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = nameof(GenerateSignUpCodes), Tags = new[] { "Sign Up Codes" })]
        public async Task<ActionResult<SignUpCodesInputDTO>> GenerateSignUpCodes([FromBody] SignUpCodesInputDTO signUpCodesInputDTO)
        {
            GenerateSignUpCodesCommand command = new GenerateSignUpCodesCommand()
            {
                SignUpCodes = signUpCodesInputDTO
            };

            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[GenerateSignUpCodes] {result.Error}");
                return ValidationProblem(result.Error);
            }
            return Ok();
        }

        /// <summary>
        /// Notify SignUp codes
        /// </summary>
        /// <param name="signupCode"></param>
        /// <param name="signUpCodesNotificationInputDTO"></param>
        /// <returns></returns>
        [HttpPost("{signupCode}/notification")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = nameof(NotifySignUpCodes), Tags = new[] { "Sign Up Codes" })]
        public async Task<ActionResult<SignUpCodesNotificationInputDTO>> NotifySignUpCodes(long signupCode, [FromBody] SignUpCodesNotificationInputDTO signUpCodesNotificationInputDTO)
        {
            NotifySignUpCodesCommand command = new NotifySignUpCodesCommand()
            {
                signUpCode = signupCode,
                signUpCodesNotification = signUpCodesNotificationInputDTO
            };

            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[NotifySignUpCodes] {result.Error}");
                return ValidationProblem(result.Error);
            }
            return Ok();
        }

        /// <summary>
        /// Retrieve sign up code info on partner registration and business profile by Id
        /// <param name="partnerCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{partnerCode}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(SignUpCodesGetByIdOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetSignUpCodeByCode), Tags = new[] { "Sign Up Codes" })]
        public async Task<IActionResult> GetSignUpCodeByCode(long partnerCode)
        {
            GetSignUpCodeByCodeQuery query = new GetSignUpCodeByCodeQuery()
            {
                PartnerCode = partnerCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[GetSignUpCodeByCode] {result.Error}");
                return ValidationProblem();
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Get Partner's Agent
        /// </summary>
        /// <param name="partnerName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{partnerName}/partner-details")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(SignUpCodesGetByPartnerNameOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetAgentByPartnerName), Tags = new[] { "Sign Up Codes" })]
        public async Task<IActionResult> GetAgentByPartnerName(string partnerName)
        {
            GetSignUpCodeAgentByPartnerNameQuery query = new GetSignUpCodeAgentByPartnerNameQuery
            {
                PartnerName = partnerName
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[GetSignUpCodeAgentByPartnerName] {result.Error}");
                return ValidationProblem();
            }
            return Ok(result.Value);
        }
    }
}
