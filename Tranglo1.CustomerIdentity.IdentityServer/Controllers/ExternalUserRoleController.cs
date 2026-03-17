using AutoMapper;
using IdentityServer4.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Security;
using System.Security.Claims;
using Tranglo1.CustomerIdentity.IdentityServer.Command;
using Tranglo1.CustomerIdentity.IdentityServer.Attributes;
using Tranglo1.CustomerIdentity.IdentityServer.Helper;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/external-user-roles")]
    [LogInputDTO]
    [LogOutputDTO]
    public class ExternalUserRoleController : ControllerBase
    {
        private readonly ILogger<ExternalUserRoleController> _logger;
        public IMediator Mediator { get; }

        public ExternalUserRoleController(ILogger<ExternalUserRoleController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            Mediator = mediator;
        }

        /// <summary>
        /// Retrieve list of external user roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(IEnumerable<ExternalRoleListOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetAllExternalUserRoles), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> GetAllExternalUserRoles()
        {
            GetAllExternalUserRoleQuery query = new GetAllExternalUserRoleQuery();

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve list of external user roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(PagedResult<ExternalUserRoleListOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetExternalUserRoles), Tags = new[] { "External User Role" })]
        public async Task<PagedResult<ExternalUserRoleListOutputDTO>> GetExternalUserRoles([FromQuery] ExternalUserRoleListInputDTO pageFilter, int pageSize, int page, string sortExpression, SortDirection sortDirection)
        {
            GetExternalUserRolesQuery query = new GetExternalUserRolesQuery
            {
                DTO = pageFilter,
                PagingOptions = new PagingOptions
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    SortExpression = sortExpression,
                    Direction = sortDirection
                }
            };

            return await Mediator.Send(query);
        }

        /// <summary>
        /// Retrieve external user role by RoleCode
        /// </summary>
        /// <returns></returns>
        [HttpGet("{roleCode}/{solutionCode}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(ExternalUserRoleOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetExternalUserRole), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> GetExternalUserRole([FromRoute] string roleCode, [FromRoute] int solutionCode)
        {
            GetExternalUserRoleQuery query = new GetExternalUserRoleQuery
            {
                RoleCode = roleCode,
                SolutionCode = solutionCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve external user role by claims value
        /// </summary>
        /// <returns></returns>
        [HttpGet("external-user-companies-roles")]
        [Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        [ProducesResponseType(typeof(ExternalUserRoleByClaimOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetExternalUserRoleByClaim), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> GetExternalUserRoleByClaim()
        {
            GetExternalUserRoleByClaimQuery query = new GetExternalUserRoleByClaimQuery
            {
                LoginId = User.GetSubjectId().Value
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve the list of connect screen access for a new external role creation
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v{version:apiVersion}/customer-screen-access/{solutionCode}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<ConnectScreenAccessOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetNewConnectScreenAccess), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> GetNewConnectScreenAccess(long solutionCode)
        {
            GetConnectScreenAccessQuery query = new GetConnectScreenAccessQuery
            {
                SolutionCode = solutionCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }

            return Ok(result.Value);
        }


        /// <summary>
        /// Retrieve the list of login connect screen access based on the role selection from landing profile
        /// </summary>
        /// <returns></returns>
        [HttpGet("{partnerCode}/{roleCode}/login-customer-screen-access")]
        [Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<UACLoginSubMenuActionOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetLoginConnectScreenAccess), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> GetLoginConnectScreenAccess(long partnerCode, string roleCode, int solutionCode)
        {
            GetLoginConnectScreenAccessQuery query = new GetLoginConnectScreenAccessQuery
            {
                RoleCode = roleCode,
                PartnerCode = partnerCode,
                SolutionCode = solutionCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Insert new External User Role
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<NewExternalUserRoleOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(SaveExternalUserRole), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> SaveExternalUserRole([FromBody] NewExternalUserRoleInputDTO userRoleInputDTO)
        {
            SaveExternalUserRoleCommand query = new SaveExternalUserRoleCommand
            {
                ExternalUserRoleInputDTO = userRoleInputDTO
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Edit existing External User Role
        /// </summary>
        /// <returns></returns>
        [HttpPut("{roleCode}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<UpdateExternalUserRoleOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(UpdateExternalUserRole), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> UpdateExternalUserRole([FromBody] UpdateExternalUserRoleInputDTO userRoleInputDTO, [FromRoute] string roleCode)
        {
            UpdateExternalUserRoleCommand query = new UpdateExternalUserRoleCommand
            {
                UpdateExternalUserRoleInputDTO = userRoleInputDTO,
                RoleCode = roleCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Update External User Role Status by Role Code
        /// </summary>
        /// <returns></returns>
        [HttpPut("{roleCode}/status")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<UpdateExternalRoleStatusOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(UpdateExternalUserRoleStatus), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> UpdateExternalUserRoleStatus([FromBody] UpdateExternalRoleStatusInputDTO roleStatusInputDTO, [FromRoute] string roleCode)
        {
            UpdateExternalRoleStatusCommand query = new UpdateExternalRoleStatusCommand
            {
                UpdateExternalRoleStatusInputDTO = roleStatusInputDTO,
                RoleCode = roleCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve list of external user roles by solution
        /// </summary>
        /// <returns></returns>
        [HttpGet("{solutionCode}/roles")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(IEnumerable<ExternalRoleListOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetExternalUserRolesBySolution), Tags = new[] { "External User Role" })]
        public async Task<IActionResult> GetExternalUserRolesBySolution(long solutionCode)
        {
            GetExternalUserRolesBySolutionQuery query = new GetExternalUserRolesBySolutionQuery
            {
                SolutionCode = solutionCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError("[{MethodName}] {Error}", ExtensionHelper.GetMethodName(), result.Error);
                return ValidationProblem();
            }
            return Ok(result.Value);
        }
    }
}