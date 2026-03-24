using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.IdentityServer.Command;
using Tranglo1.CustomerIdentity.IdentityServer.DTO;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Security;
using System.Security.Claims;
using AutoMapper.Configuration;
using Tranglo1.CustomerIdentity.IdentityServer.Models;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser;
using Tranglo1.CustomerIdentity.IdentityServer.Attributes;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;
using Tranglo1.CustomerIdentity.Domain.Events;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers
{
    [ApiController]
    [Route("/api/v{version:apiVersion}")]
    [ApiVersion("1")]
    [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
    [LogInputDTO]
    [LogOutputDTO]
    public class CustomerUserController : ControllerBase
    {
        public CustomerUserController(IMapper mapper, IMediator mediator, ILogger<CustomerUserController> logger)
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
        public ILogger<CustomerUserController> _logger { get; }


        ///// <summary>
        ///// Locks User under the same Business Profile
        ///// </summary>
        ///// <param name="businessProfileCode"></param>
        ///// <param name="lockUserInputDTO"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("business-profiles/{businessProfileCode}/users/lock")]
        //[Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        //[ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        //[SwaggerOperation(OperationId = nameof(LockUser), Tags = new[] { "Customer User Management" })]
        //public async Task<IActionResult> LockUser(
        //    [BusinessProfileId] int businessProfileCode,
        //    [FromBody] LockUserInputDTO lockUserInputDTO)
        //{
        //    LockUserCommand command = _mapper.Map<LockUserCommand>(lockUserInputDTO);
        //    IdentityResult identityResult = await Mediator.Send(command);

        //    if (!identityResult.Succeeded)
        //    {
        //        foreach (IdentityError item in identityResult.Errors)
        //        {
        //            ModelState.AddModelError("Error", item.Description);
        //        }

        //        _logger.LogError(nameof(LockUser), identityResult.Errors);
        //        return ValidationProblem();
        //    }
        //    return Ok();
        //}

        ///// <summary>
        ///// Unlocks User under the same Business Profile
        ///// </summary>
        ///// <param name="businessProfileCode"></param>
        ///// <param name="unlockUserInputDTO"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("business-profiles/{businessProfileCode}/users/unlock")]
        //[Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        //[ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        //[SwaggerOperation(OperationId = nameof(UnlockUser), Tags = new[] { "Customer User Management" })]
        //public async Task<IActionResult> UnlockUser(
        //    [BusinessProfileId] int businessProfileCode,
        //    [FromBody] UnlockUserInputDTO unlockUserInputDTO)
        //{
        //    try
        //    {
        //        UnlockUserCommand command = _mapper.Map<UnlockUserCommand>(unlockUserInputDTO);
        //        IdentityResult identityResult = await Mediator.Send(command);


        //        if (!identityResult.Succeeded)
        //        {
        //            foreach (IdentityError item in identityResult.Errors)
        //            {
        //                ModelState.AddModelError("Error", item.Description);
        //            }

        //            _logger.LogError(nameof(UnlockUser), identityResult.Errors);
        //            return ValidationProblem();
        //        }
        //        return Ok();
        //    }
        //   catch(Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        /// <summary>
        /// Updates the Account Status for Customer User Account Status
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <param name="email"></param>
        /// <param name="updateCustomerUserAccountStatus"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("business-profiles/{businessProfileCode}/users/{email}/account-statuses")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(UpdateCustomerUserBusinessProfileAccountStatus), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> UpdateCustomerUserBusinessProfileAccountStatus(
            [BusinessProfileId] int businessProfileCode,
            [EmailAddress] string email,
            [FromBody] UpdateCustomerUserAccountStatusInputDTO updateCustomerUserAccountStatus)
        {
            try
            {
                CustomerUserBusinessProfileAccountStatusCommand query = new CustomerUserBusinessProfileAccountStatusCommand
                {
                    BusinessProfileCode = businessProfileCode,
                    AccountStatusInput = updateCustomerUserAccountStatus,
                    Email = email
                };

                var result = await Mediator.Send(query);


                if (result.IsFailure)
                {

                    ModelState.AddModelError("Error", result.Error);             
                    _logger.LogError($"[UpdateCustomerUserBusinessProfileAccountStatus] {result.Error}");
                    return ValidationProblem();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates the Account Status for Customer User Account Status
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <param name="email"></param>
        /// <param name="updateCustomerUserBlockStatus"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("business-profiles/{businessProfileCode}/users/{email}/block-statuses")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(UpdateCustomerUserBusinessProfileBlockStatus), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> UpdateCustomerUserBusinessProfileBlockStatus(
            [BusinessProfileId] int businessProfileCode,
           [EmailAddress] string email,
            [FromBody] UpdateCustomerUserBlockStatusInputDTO updateCustomerUserBlockStatus)
        {
            try
            {
                CustomerUserBusinessProfileBlockStatusCommand query = new CustomerUserBusinessProfileBlockStatusCommand
                {
                    BusinessProfileCode = businessProfileCode,
                    BlockStatusInput = updateCustomerUserBlockStatus,
                    Email = email
                };

                IdentityResult identityResult = await Mediator.Send(query);
                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError item in identityResult.Errors)
                    {
                        ModelState.AddModelError("Error", item.Description);
                    }

                    _logger.LogError($"[UpdateCustomerUserBusinessProfileBlockStatus] {identityResult.Errors}");
                    return ValidationProblem();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
        /*
        /// <summary>
        /// Display the list of customer users under the same Business Profile
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <param name="name"></param>
        /// <param name="environment"></param>
        /// <param name="roleCode"></param>
        /// <param name="statusCode"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="sortExpression"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("business-profiles/{businessProfileCode}/users")]
        [Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(Search), Tags = new[] { "Customer User Management" })]
        //[Authorize(Roles = "Master Admin")] //TODO: Enable 
        public async Task<PagedResult<TeamUserListOutputDTO>> Search(int businessProfileCode, string name,
           string environment, string roleCode, int statusCode, int pageSize, int page, string sortExpression, SortDirection sortDirection)
        {
            GetTeamUserListQuery query = new GetTeamUserListQuery
            {
                UserId = User.GetUserId().Value,
                BusinessProfileCode = businessProfileCode,
                Environment = environment,
                Name = name,
                PageIndex = page,
                PageSize = pageSize,
                RoleCode = roleCode,
                StatusCode = statusCode,
                SortExpression = sortExpression,
                Direction = sortDirection
            };

            return await Mediator.Send(query);
        }
        */
        /*
        /// <summary>
        /// Retrieve Invitation roles listing under a business profile, which excludes Master Admin
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("business-profiles/{businessProfileCode}/invitation-roles")]
        //[Authorize(Roles = "Master Admin")] //TODO: Enable
        [Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        [ProducesResponseType(typeof(IReadOnlyList<RolesAvailableToInviteeOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetUserRolesAvailableToInvitee), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> GetUserRolesAvailableToInvitee([BusinessProfileId] int businessProfileCode)
        {
            GetRolesAvailableToInviteeQuery query = new GetRolesAvailableToInviteeQuery
            {
                BusinessProfileCode = businessProfileCode
            };


            var result = await Mediator.Send(query);

            return Ok(result.Value);
        }
        */
        /// <summary>
        /// Invite user to the same business profile of the inviter. This will create a new invitee customer user for login
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("business-profiles/{businessProfileCode}/users/invite")]
        //[Authorize(Roles = "Master Admin")] //TODO: Enable
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(InviteUser), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> InviteUser(int businessProfileCode, [FromBody] InviteUserInputDTO model)
        {
            InviteUserCommand command = _mapper.Map<InviteUserCommand>(model);

            //Maybe<string> _InviterEmail = User.GetUserEmail();
            //if (_InviterEmail.HasNoValue)
            //{
            //    //should not happened this
            //    _logger.LogWarning("Inviter email not found when submitting invitations.");
            //    ModelState.AddModelError("InviterEmail", "Inviter email not found.");
            //    return ValidationProblem();
            //}
            //command.InviterEmail = _InviterEmail.Value;

            string LoginId = User.GetSubjectId().Value;
            command.LoginId = LoginId;
            command.BusinessProfileCode = businessProfileCode;

            IdentityResult result = await Mediator.Send(command);

            if (!result.Succeeded)
            {
                foreach (IdentityError item in result.Errors)
                {
                    ModelState.AddModelError("Error", item.Description);
                }

                _logger.LogError($"[InviteUser] {result.Errors}");
                return ValidationProblem();
            }

            return Ok(IdentityResult.Success);
        }

        /// <summary>
        /// Resends invitation to the new customer user
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <param name="inviteeEmail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{businessProfileCode}/users/{inviteeEmail}/resend-invitation")]
        //[Authorize(Roles = "Master Admin")] //TODO: Enable 
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(ResendInvitation), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> ResendInvitation(int businessProfileCode, string inviteeEmail)
        {
            ResendInvitationCommand command = new ResendInvitationCommand();

            //Maybe<string> _InviterEmail = User.GetUserEmail();

            //if (LoginId.HasNoValue)
            //{
            //    //should not happened this
            //    _logger.LogWarning("Inviter email not found when submitting invitations.");
            //    ModelState.AddModelError("LoginId", "Login Id not found.");
            //    return ValidationProblem();
            //}

            //command.InviterEmail = _InviterEmail.Value;
            string LoginId = User.GetSubjectId().Value;
            command.LoginId = LoginId;
            command.BusinessProfileCode = businessProfileCode;
            command.InviteeEmail = inviteeEmail;
            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
            {
               ModelState.AddModelError("Error", result.Error);
                _logger.LogError($"[ResendInvitation] {result.Error}");
                return ValidationProblem();
            }

            return Ok(Result.Success());
        }

        /// <summary>
        /// resend user email verification.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>RedirectResult</returns>
        [HttpPost]
        [Route("users/{email}/resend-verification")]
        public async Task<IActionResult> ResendEmailVerification(string email)
        {
            ResendEmailVerificationCommand command = new ResendEmailVerificationCommand();
            {
                command.Email = email;

            };
            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);
                return ValidationProblem(result.Error);
            }
            return Ok(result);
        }

        /*
        /// <summary>
        /// Invite user to the same business profile of the inviter. This will create a new invitee customer user for login
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("business-profiles/{businessProfileCode}/roles")]
        //[Authorize(Roles = "Master Admin")] //TODO: Enable
        [Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        [ProducesResponseType(typeof(IReadOnlyList<RolesAvailableToInviteeOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetRolesByBusinessProfile), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> GetRolesByBusinessProfile([BusinessProfileId] int businessProfileCode)
        {
            GetRolesByBusinessProfileQuery query = new GetRolesByBusinessProfileQuery
            {
                BusinessProfileCode = businessProfileCode
            };

            var result = await Mediator.Send(query);

            return Ok(result.Value);
        }
        */


        /// <summary>
        /// Customer - Retrieve Partner User Details
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("business-profiles/{businessProfileCode}/users/{email}")]
        [Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<UpdatePartnerUserCustomerOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetPartnerUserDetailsCustomer), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> GetPartnerUserDetailsCustomer([FromRoute] int businessProfileCode, string email)
        {
            GetPartnerUserDetailsCustomerQuery query = new GetPartnerUserDetailsCustomerQuery()
            {
                BusinessProfileCode = businessProfileCode,
                Email = email
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetPartnerUserDetailsCustomer] {result.Error}");
                return ValidationProblem(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Admin - Retrieve Partner User Details
        /// </summary>
        /// <returns></returns>
        [HttpGet("users/{email}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(IEnumerable<ViewPartnerUserAdminOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetPartnerUserDetailsAdmin), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> GetPartnerUserDetailsAdmin(string email, long customerUserBusinessProfileCode, int businessProfileCode)
        {
            GetPartnerUserDetailsAdminQuery query = new GetPartnerUserDetailsAdminQuery()
            {
                Email = email,
                CustomerUserBusinessProfileCode = customerUserBusinessProfileCode,
                BusinessProfileCode = businessProfileCode
            };

            var result = await Mediator.Send(query);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[GetPartnerUserDetailsAdmin] {result.Error}");
                return ValidationProblem(result.Error);
            }

            return Ok(result.Value);
        }


        /// <summary>
        /// Customer - Update Partner User Details
        /// </summary>
        /// <returns></returns>
        [HttpPut("business-profiles/{businessProfileCode}/users/{email}")]
        [Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        [ProducesResponseType(typeof(UpdatePartnerUserCustomerInputDTO), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = nameof(UpdatePartnerUserDetailsCustomer), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> UpdatePartnerUserDetailsCustomer(string email, int businessProfileCode ,[FromBody] UpdatePartnerUserCustomerInputDTO individualPartnerUser)
        {
            UpdatePartnerDetailsCustomerCommand command = new UpdatePartnerDetailsCustomerCommand()
            {
                Email = email,
                BusinessProfileCode = businessProfileCode,
                DTO = individualPartnerUser
            };

            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[UpdatePartnerUserDetailsCustomer] {result.Error}");
                return ValidationProblem(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Admin - Update Partner User Details
        /// </summary>
        /// <returns></returns>
        [HttpPut("users/{email}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(UpdatePartnerUserAdminInputDTO), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = nameof(UpdatePartnerUserDetailsAdmin), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> UpdatePartnerUserDetailsAdmin(string email, [FromBody] UpdatePartnerUserAdminInputDTO individualPartnerUser)
        {
            UpdatePartnerDetailsAdminCommand command = new UpdatePartnerDetailsAdminCommand()
            {
                Email = email,
                DTO = individualPartnerUser
            };

            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Error", result.Error);

                _logger.LogError($"[UpdatePartnerUserDetailsAdmin] {result.Error}");
                return ValidationProblem(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Customer - Display list of Customer Users per Business Profile
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <param name="name"></param>
        /// <param name="userRoleCode"></param>
        /// <param name="accountStatusCode"></param>
        /// <param name="userEnvironmentCode"></param>
        /// <param name="companyAccountStatusFilter"></param>
        /// <param name="companyBlockStatusFilter"></param>
        /// <param name="nameOrEmailFilter"></param>
        /// <param name="multipleRoleFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="sortExpression"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        [HttpGet("business-profiles/{businessProfileCode}/users")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetPartnerUserListCustomer), Tags = new[] { "Customer User Management" })]
        //[Authorize(Roles = "Master Admin")] //TODO: Enable 
        public async Task<PagedResult<PartnerUserListCustomerOutputDTO>> GetPartnerUserListCustomer(int businessProfileCode, string name,
           string userRoleCode, int? accountStatusCode, int userEnvironmentCode, string companyAccountStatusFilter, string companyBlockStatusFilter, string nameOrEmailFilter, string multipleRoleFilter, 
           int pageSize, int page, string sortExpression, SortDirection sortDirection)
        {
            GetPartnerUserListCustomerQuery query = new GetPartnerUserListCustomerQuery
            {
                BusinessProfileCode = businessProfileCode,
                PageIndex = page,
                PageSize = pageSize,
                NameFilter = name,
                UserRoleFilter = userRoleCode,
                AccountStatusFilter = accountStatusCode,
                UserEnvironmentFilter = userEnvironmentCode,
                CompanyAccountStatusFilter = companyAccountStatusFilter,
                CompanyBlockStatusFilter = companyBlockStatusFilter,
                NameOrEmailFilter = nameOrEmailFilter,
                MultipleRoleFilter = multipleRoleFilter,
                SortExpression = sortExpression,
                Direction = sortDirection
            };

            return await Mediator.Send(query);
        }

        /// <summary>
        /// Admin - Display list of customer users 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="solutionCode"></param>
        /// <param name="email"></param>
        /// <param name="userRoleCode"></param>
        /// <param name="accountStatusCode"></param>
        /// <param name="company"></param>
        /// <param name="userEnvironmentCode"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="sortExpression"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        [HttpGet("users")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetPartnerUserListAdmin), Tags = new[] { "Customer User Management" })]
        //[Authorize(Roles = "Master Admin")] //TODO: Enable 
        public async Task<PagedResult<PartnerUserListAdminOutputDTO>> GetPartnerUserListAdmin(string name, long? solutionCode, string email,
           string userRoleCode, int? accountStatusCode, int? company, int? userEnvironmentCode, int pageSize, int page, string sortExpression, SortDirection sortDirection)
        {
            GetPartnerUserListAdminQuery query = new GetPartnerUserListAdminQuery
            {
                NameFilter = name,
                SolutionFilter = solutionCode,
                EmailFilter = email,
                UserRoleFilter = userRoleCode,
                CompanyFilter = company,
                AccountStatusFilter = accountStatusCode,
                UserEnvironmentFilter = userEnvironmentCode,
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
        /// Retrieve list of Business Profiles per Customer User
        /// </summary>
        /// <returns></returns>
        [HttpGet("users/{email}/business-profiles/{businessProfileCode}/customer-user-registration/{customerUserRegistrationId}")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [SwaggerOperation(OperationId = nameof(GetBusinessProfileListPerCustomerUser), Tags = new[] { "Customer User Management" })]
        public async Task<List<GetBusinessProfileListPerCustomerUserOutputDTO>> GetBusinessProfileListPerCustomerUser(string email, int businessProfileCode, int customerUserRegistrationId)
        {
            GetBusinessProfileListPerCustomerUserQuery query = new GetBusinessProfileListPerCustomerUserQuery()
            { 
                Email = email,
                BusinessProfileCode = businessProfileCode,
                CustomerUserRegistrationId = customerUserRegistrationId
            };

            return await Mediator.Send(query);
        }

        /// <summary>
        /// Retrieve list of all Business Profiles
        /// </summary>
        /// <returns></returns>
        [HttpGet("business-profiles")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [SwaggerOperation(OperationId = nameof(GetAllBusinessProfiles), Tags = new[] { "Customer User Management" })]
        public async Task<List<GetAllBusinessProfilesOutputDTO>> GetAllBusinessProfiles()
        {
            GetAllBusinessProfilesQuery query = new GetAllBusinessProfilesQuery();
            return await Mediator.Send(query);
        }

        /// <summary>
        /// Customer - Display Customer User Tranglo Entity by Partner Code per Business Profile
        /// </summary>
        /// <param name="businessProfileCode"></param>
        
        /// <returns></returns>
        [HttpGet("business-profiles/{businessProfileCode}/users/tranglo-entities")]
        [Authorize(Policy = AuthenticationPolicies.ExternalOnlyPolicy)]
        [ProducesResponseType(typeof(GetPartnerUserTrangloEntityOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetPartnerUserTrangloEntity), Tags = new[] { "Customer User Management" })]
        public async Task<Result<GetPartnerUserTrangloEntityOutputDTO>> GetPartnerUserTrangloEntity(int businessProfileCode)
        {
            var solution = System.Security.Claims.ClaimsPrincipalExtensions.GetSolutionCode(User); // convert Maybe<string> to string

            GetPartnerUserTrangloEntityQuery query = new GetPartnerUserTrangloEntityQuery
            {
                BusinessProfileCode = businessProfileCode,
                CustomerSolution = solution.HasValue ? solution.Value : null

            };


            return await Mediator.Send(query);
        }

        /// <summary>
        /// Get Partner's  Invite Solution
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{businessProfileCode}/partner-invite-detail")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(PartnerInviteSolutionByBusinessProfileOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetSolutionByBusinessProfileCode), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> GetSolutionByBusinessProfileCode(long businessProfileCode)
        {
            GetPartnerInviteSolutionByBusinessProfileCodeQuery query = new GetPartnerInviteSolutionByBusinessProfileCodeQuery
            {
                BusinessProfileCode = businessProfileCode
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

        /// <summary>
        /// Get Partner's  Invite Solution
        /// </summary>
        /// <param name="businessProfileCode"></param>
        /// <returns></returns>
        [HttpGet("{businessProfileCode}/registered-country-details")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(CustomerUserRegisteredCountryOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(GetCustomerRegisteredCountryByBusinessProfileCode), Tags = new[] { "Customer User Management" })]
        public async Task<IActionResult> GetCustomerRegisteredCountryByBusinessProfileCode(int businessProfileCode)
        {
            var query = new GetCustomerRegisteredCountryByBusinessProfileCodeQuery()
            {
                BusinessProfileCode = businessProfileCode
            };
            var result = await Mediator.Send(query);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(result.Value);

        }

    }
}
