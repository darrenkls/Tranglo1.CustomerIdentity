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
using Tranglo1.CustomerIdentity.IdentityServer.Command;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Security;
using System.Security.Claims;
using Tranglo1.CustomerIdentity.IdentityServer.Attributes;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/tranglostaffs")]
    [LogInputDTO]
    [LogOutputDTO]

    public class TrangloStaffController : ControllerBase
    {
        public TrangloStaffController(IMapper mapper, IMediator mediator, ILogger<TrangloStaffController> logger)
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
        public ILogger<TrangloStaffController> _logger { get; }


        /// <summary>
        /// Retrieve list of Tranglo Staff
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<TrangloUserGroupingOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetAdminUsers), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> GetAdminUsers(string name, string role, string entity, int? accountStatusId, int? department,
            int pageSize, int pageIndex)
        {
            if (accountStatusId == 0)
            {
                accountStatusId = null;
            }
            if (department == 0)
            {
                department = null;
            }
            GetAdminUsersQuery query = new GetAdminUsersQuery
            {
                Name = name,
                Role = role,
                Entity = entity,
                AccountStatus = accountStatusId,
                Department = department,
                PagingOptions = new PagingOptions
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };

            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetAdminUsers] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves a Tranglo Staff user by LoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet("{loginId}")]
        [ProducesResponseType(typeof(IEnumerable<TrangloStaffOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetTrangloStaffUser), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> GetTrangloStaffUser(string loginId)
        {
            GetTrangloStaffByLoginIdQuery query = new GetTrangloStaffByLoginIdQuery
            {
                LoginId = loginId
            };

            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetTrangloStaffUser] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves a Tranglo Staff user by claims value
        /// </summary>
        /// <returns></returns>
        [HttpGet("tranglo-user-entities-roles")]
        [ProducesResponseType(typeof(IEnumerable<TrangloStaffByClaimsOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetTrangloStaffByClaims), Tags = new[] { "Tranglo Staff" })]

        public async Task<IActionResult> GetTrangloStaffByClaims()
        {
            GetTrangloStaffByClaimsQuery query = new GetTrangloStaffByClaimsQuery
            {
                LoginId = User.GetSubjectId().Value
            };

            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetTrangloStaffByClaims] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets a list of Active Directory Accounts that is not a Tranglo Staff
        /// </summary>
        /// <returns></returns>
        [HttpGet("ad-accounts")]
        [ProducesResponseType(typeof(IEnumerable<GetUnassignedADAccountOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetUnassignedADAccount), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> GetUnassignedADAccount()
        {
            GetUnassignedADAccountQuery query = new GetUnassignedADAccountQuery
            {

            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetUnassignedADAccount] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieve list of assigned Compliance Officers
        /// </summary>
        /// <returns></returns>
        [HttpGet("compliance-officers")]
        [ProducesResponseType(typeof(IEnumerable<KYCComplianceOfficerOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetKYCComplianceOfficer), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> GetKYCComplianceOfficer()
        {
            GetKYCComplianceOfficerQuery query = new GetKYCComplianceOfficerQuery
            {

            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetKYCComplianceOfficer] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        [HttpGet("product")]
        [ProducesResponseType(typeof(IEnumerable<KYCProductStaffOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetTrangloProductStaff), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> GetTrangloProductStaff()
        {
            GetTrangloProductStaffQuery query = new GetTrangloProductStaffQuery
            {

            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetTrangloProductStaff] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        [HttpGet("sales-operation")]
        [ProducesResponseType(typeof(IEnumerable<KYCSalesOperationStaffOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetTrangloSalesOperationStaff), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> GetTrangloSalesOperationStaff()
        {
            GetTrangloSalesOperationStaffQuery query = new GetTrangloSalesOperationStaffQuery
            {

            };

            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetTrangloSalesOperationStaff] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        [HttpGet("business-development")]
        [ProducesResponseType(typeof(IEnumerable<KYCBusinessDevelopmentStaffOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetTrangloBusinessDevelopmentStaff), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> GetTrangloBusinessDevelopmentStaff()
        {
            GetTrangloBusinessDevelopmentStaffQuery query = new GetTrangloBusinessDevelopmentStaffQuery
            {

            };

            var result = await Mediator.Send(query);
            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetTrangloBusinessDevelopmentStaff] {result.Error}");
                return ValidationProblem();
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates a Tranglo Staff User (Role, Department, etc.)
        /// </summary>
        /// <returns></returns>
        [HttpPut("{loginId}")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(EditTrangloStaffUser), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> EditTrangloStaffUser(string loginId, [FromBody] TrangloStaffUserUpdateInputDTO trangloStaffUser)
        {
            UpdateTrangloStaffAssignmentCommand command = _mapper.Map<UpdateTrangloStaffAssignmentCommand>(trangloStaffUser);
            command.roleDepartmentEntityInput = trangloStaffUser.trangloStaffEntity;
            command.Name = trangloStaffUser.Name;
            command.Emails = trangloStaffUser.Email;
            command.AccountStatus = trangloStaffUser.AccountStatus;
            command.Timezone = trangloStaffUser.Timezone;
            command.LoginId = loginId;
            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[EditTrangloStaffUser] {result.Error}");
                return ValidationProblem(result.Error);
            }

            return Ok(result);
        }

        /// <summary>
        /// Adds a new Tranglo Staff User
        /// </summary>
        /// <returns></returns>
        [HttpPost("{loginId}")]
        [ProducesResponseType(typeof(AddTrangloStaffOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(AddTrangloStaffUser), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> AddTrangloStaffUser(string loginId, [FromBody] AddTrangloStaffInputDTO addTrangloStaff)
        {
            SaveTrangloStaffCommand query = new SaveTrangloStaffCommand
            {
                LoginId = loginId,
                AddTrangloStaff = addTrangloStaff

            };
            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[AddTrangloStaffUser] {result.Error}");
                return ValidationProblem(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates Block Status for Tranglo Entity
        /// </summary>
        /// <returns></returns>
        [HttpPut("{loginId}/tranglo-entities/{trangloEntity}/block-statuses")]
        [ProducesResponseType(typeof(TrangloEntityBlockStatusOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(UpdateTrangloEntityBlockStatus), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> UpdateTrangloEntityBlockStatus(string loginId, string trangloEntity, [FromBody] TrangloEntityBlockStatusInputDTO trangloEntityBlockStatus)
        {

            UpdateTrangloStaffBlockStatusCommand query = new UpdateTrangloStaffBlockStatusCommand
            {
                LoginId = loginId,
                BlockStatusInput = trangloEntityBlockStatus,
                TrangloEntity = trangloEntity
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[UpdateTrangloEntityBlockStatus] {result.Error}");
                return ValidationProblem(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates Account Status for Tranglo Staff
        /// </summary>
        /// <returns></returns>
        [HttpPut("{loginId}/account-status")]
        [ProducesResponseType(typeof(TrangloStaffAccountStatusOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(UpdateTrangloStaffAccountStatus), Tags = new[] { "Tranglo Staff" })]
        public async Task<IActionResult> UpdateTrangloStaffAccountStatus(string loginId, [FromBody] TrangloStaffAccountStatusInputDTO trangloEntityBlockStatus)
        {

            UpdateTrangloStaffAccountStatusCommand query = new UpdateTrangloStaffAccountStatusCommand
            {
                loginId = loginId,
                accountStatusInput = trangloEntityBlockStatus
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[UpdateTrangloStaffAccountStatus] {result.Error}");
                return ValidationProblem(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
