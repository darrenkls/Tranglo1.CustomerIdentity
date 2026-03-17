using AutoMapper;
using CSharpFunctionalExtensions;
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
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.IdentityServer.Attributes;
using Tranglo1.CustomerIdentity.IdentityServer.Command;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.TrangloRole;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Security;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers.TrangloRole
{
    [ApiController]
    [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}")]
    [LogInputDTO]
    [LogOutputDTO]
    public class TrangloRoleController : ControllerBase
    {
        public TrangloRoleController(IMapper mapper, IMediator mediator, ILogger<TrangloRoleController> logger)
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (mediator is null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _mapper = mapper;
            Mediator = mediator;
            _logger = logger;
        }

        public IMapper _mapper { get; }
        public IMediator Mediator { get; }
        public ILogger<TrangloRoleController> _logger { get; }




        /// <summary>
        /// Retrieves all the Roles in a Department
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<GetTrangloRoleByDeptListOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpGet("departments/{departmentId}/tranglo-roles")]
        [SwaggerOperation(OperationId = nameof(GetTrangloRoleByDepartmentList), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetTrangloRoleByDepartmentList(int departmentId)
        {
            GetTrangloRoleByDeptListQuery query = new GetTrangloRoleByDeptListQuery
            {
                DepartmentId = departmentId
            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetTrangloRoleByDepartmentList] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves all the Roles of a Creator by Creator Role Code
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(PagedResult<GetTrangloRoleListOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpGet("tranglo-role-list")]
        [SwaggerOperation(OperationId = nameof(GetTrangloRoleList), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetTrangloRoleList(string creatorRoleCode, string roleName, int? departmentCode, int? roleStatusCode,
            int? authorityLevelCode, bool? isSuperApprover, int pageSize, int page)
        {
            GetTrangloRoleListQuery query = new GetTrangloRoleListQuery
            {
                CreatorRoleCode = creatorRoleCode,
                RoleName = roleName,
                DepartmentCode = departmentCode,
                RoleStatusCode = roleStatusCode,
                AuthorityLevelCode = authorityLevelCode,
                IsSuperApprover = isSuperApprover,
                PagingOptions = new PagingOptions
                {
                    PageIndex = page,
                    PageSize = pageSize
                }
            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetTrangloRolesList] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Get All Admin ACL List
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<AdminScreenAccessOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpGet("tranglo-roles/admin-screen-access")]
        [SwaggerOperation(OperationId = nameof(GetAdminScreenAccessList), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetAdminScreenAccessList()
        {
            GetAdminScreenAccessListQuery query = new GetAdminScreenAccessListQuery();

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetAdminScreenAccessList] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);

        }


        /// <summary>
        /// Retrieves Tranglo Role by Role Code
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(GetTrangloRoleByRoleCodeOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpGet("tranglo-roles")]
        [SwaggerOperation(OperationId = nameof(GetTrangloRoleByRoleCode), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetTrangloRoleByRoleCode(string roleCode)
        {
            GetTrangloRoleByRoleCodeQuery query = new GetTrangloRoleByRoleCodeQuery
            {
                RoleCode = roleCode
            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetTrangloRoleByRoleCode] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Update Tranglo Role by Role Code
        /// </summary>
        /// <param name="roleCode"></param>
        /// <param name="updateTrangloRole"></param>
        /// <returns></returns>
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(UpdateTrangloRoleOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpPut("tranglo-roles/{roleCode}")]
        [SwaggerOperation(OperationId = nameof(UpdateTrangloRoleByCode), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> UpdateTrangloRoleByCode(string roleCode, [FromBody] UpdateTrangloRoleInputDTO updateTrangloRole)
        {
            UpdateTrangloRoleCommand query = new UpdateTrangloRoleCommand
            {
                RoleCode = roleCode,
                UpdateTrangloRole = updateTrangloRole
            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[UpdateTrangloRoleByCode] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Update Tranglo Role Status by Role Code
        /// </summary>
        /// <param name="roleCode"></param>
        /// <param name="updateTrangloRoleStatus"></param>
        /// <returns></returns>
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(UpdateTrangloRoleStatusOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpPut("tranglo-roles/{roleCode}/status")]
        [SwaggerOperation(OperationId = nameof(UpdateTrangloRoleStatus), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> UpdateTrangloRoleStatus(string roleCode, [FromBody] UpdateTrangloRoleStatusInputDTO updateTrangloRoleStatus)
        {
            UpdateTrangloRoleStatusCommand query = new UpdateTrangloRoleStatusCommand
            {
                RoleStatusInputDTO = updateTrangloRoleStatus,
                RoleCode = roleCode
            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[UpdateTrangloRoleStatus] {result.Error}");
                return ValidationProblem(result.Error);
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Insert new Tranglo Role
        /// </summary>
        /// <param name="creatorRoleCode"></param>
        /// <param name="addTrangloRole"></param>
        /// <returns></returns>
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(AddTrangloRoleOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpPost("tranglo-roles/{creatorRoleCode}")]
        [SwaggerOperation(OperationId = nameof(SaveTrangloRole), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> SaveTrangloRole(string creatorRoleCode, [FromBody] AddTrangloRoleInputDTO addTrangloRole)
        {

            AddTrangloRoleCommand query = new AddTrangloRoleCommand
            {
                CreatorRoleCode = creatorRoleCode,
                AddTrangloRoleInput = addTrangloRole
            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[SaveTrangloRole] {result.Error}");
                return ValidationProblem(result.Error);
            }
            return Ok(result.Value);

        }

        /// <summary>
        /// Retrieve the list of login admin screen access based on the role selection from landing profile
        /// </summary>
        /// <returns></returns>
        [HttpGet("tranglo-roles/{roleCode}/login-admin-screen-access")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetLoginAdminScreenAccess), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetLoginAdminScreenAccess(string roleCode)
        {
            //STEP 1: CROSS CHECK IF THE ROLE IS WITHIN THE CLAIMS
            //STEP 2: GET THE JSON FILE NAME FROM THE CLAIMS
            //STEP 3: GRAB THE LIST OF ACCESS LIST FROM THE JSON FILE / DATABASE BASED ON THE THE ROLE if step 2 & 3 is okay
            GetLoginConnectScreenAccessQuery query = new GetLoginConnectScreenAccessQuery
            {
                RoleCode = roleCode,
                SolutionCode = (int)PortalCode.Admin
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetLoginAdminScreenAccess] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve the list of Level 1 Approvers
        /// </summary>
        /// <returns></returns>
        [HttpGet("tranglo-roles/l1-approver-list")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetL1ApproverList), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetL1ApproverList()
        {
            GetL1ApproverListQuery query = new GetL1ApproverListQuery
            {

            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetL1ApproverListQuery] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve the list of Level 2 Approvers
        /// </summary>
        /// <returns></returns>
        [HttpGet("tranglo-roles/l2-approver-list")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetL2ApproverList), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetL2ApproverList()
        {
            GetL2ApproverListQuery query = new GetL2ApproverListQuery
            {

            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetL2ApproverListQuery] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve the list of Compliance Level 2 Approvers
        /// </summary>
        /// <returns></returns>
        [HttpGet("tranglo-roles/compliance-l2-approver-list")]
        [AllowAnonymous]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetComplianceL2ApproverList), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetComplianceL2ApproverList(string _trangloEntity)
        {
            GetComplianceL2ApproverListQuery query = new GetComplianceL2ApproverListQuery
            {
                trangloEntity = _trangloEntity
            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetComplianceL2ApproverListQuery] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve the list of Compliance email
        /// </summary>
        /// <returns></returns>
        [HttpGet("tranglo-roles/compliance-email-list")]
        [AllowAnonymous]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetComplianceEmailList), Tags = new[] { "Tranglo Role" })]
        public async Task<IActionResult> GetComplianceEmailList(int businessProfileCode)
        {
            GetComplianceEmailListQuery query = new GetComplianceEmailListQuery
            {
                businessProfileCode = businessProfileCode
            };
            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError("[GetComplianceEmailList] {Error}", result.Error);
                return ValidationProblem();
            }

            return Ok(result.Value);
        }
    }
}
