using IdentityServer4.Extensions;
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
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Security;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/")]
    public class UserAccessControlController: ControllerBase
    {
        private readonly ILogger<UserAccessControlController> _logger;
        public IMediator Mediator { get; }

        public UserAccessControlController(ILogger<UserAccessControlController> logger, IMediator mediator)
        {
            _logger = logger;
            Mediator = mediator;
        }

        /// <summary>
        /// Retrieve the list of login connect screen access based on the role selection from landing profile
        /// </summary>
        /// <returns></returns>
        [HttpGet("user-access-right/{permissionCode}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetAccessRightCheck), Tags = new[] { "User Access Right" })]
        public async Task<IActionResult> GetAccessRightCheck(string permissionCode)
        {
            var roleCode_claims =  User.Claims.FirstOrDefault(x => x.Type.Contains("role_code"));
            List<string> roleCodes = new List<string>();

            if(roleCode_claims !=null)
                roleCodes.AddRange(roleCode_claims?.Value?.Split(","));

            GetUserAccessControlCheckQuery query = new GetUserAccessControlCheckQuery
            {
                RoleCodes = roleCodes,
                PermissionCode = permissionCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetAccessRightCheck] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }
    }
}
