using CorrelationId.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Attributes;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.MFA;
using Tranglo1.CustomerIdentity.IdentityServer.Security;
using Tranglo1.CustomerIdentity.IdentityServer.Services;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers.MFA
{
    [ApiController]
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/MFA")]
    [LogInputDTO]
    [LogOutputDTO]
    public class MFAController : ControllerBase
    {
        private readonly ILogger<MFAController> _logger;
        public IMediator _mediator { get; }
        public readonly TrangloUserManager _userManager;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly UrlEncoder _urlEncoder;
        private readonly IHostEnvironment _env;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private readonly IAuditLogService _auditLogService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ResetMFASuccessfulEmailSender _resetMFASuccessfulEmailSender;

        public MFAController(
            TrangloUserManager userManager,
            ILogger<MFAController> logger,
            IMediator mediator,
            IApplicationUserRepository applicationUserRepository,
            UrlEncoder urlEncoder,
            IHostEnvironment env,
            ICorrelationContextAccessor correlationContextAccessor,
            IAuditLogService auditLogService,
            SignInManager<ApplicationUser> signInManager,
            ResetMFASuccessfulEmailSender resetMFASuccessfulEmailSender
            )
        {
            _userManager = userManager;
            _logger = logger;
            _mediator = mediator;
            _applicationUserRepository = applicationUserRepository;
            _urlEncoder = urlEncoder;
            _env = env;
            _correlationContextAccessor = correlationContextAccessor;
            _auditLogService = auditLogService;
            _signInManager = signInManager;
            _resetMFASuccessfulEmailSender = resetMFASuccessfulEmailSender;
        }

        [HttpGet("enabled-authentication")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [SwaggerOperation(OperationId = nameof(EnabledAuthenticatorApp), Tags = new[] { "MFA" })]
        public async Task<IActionResult> EnabledAuthenticatorApp([FromQuery] string loginId)
        {
            var user = await _userManager.FindByIdAsync(loginId);

            var mfa = await _applicationUserRepository.GetMFAAsync(user);

            var response = new
            {
                isEmail = mfa?.AuthenticationType == AuthenticationType.Email,
                isAuthenticatorApp = mfa?.AuthenticationType == AuthenticationType.Authenticator_Application
            };

            return Ok(response);
        }

        [HttpGet("setup/get-multi-factor-authenticator")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [SwaggerOperation(OperationId = nameof(SetupMultiFactorAuthenticator), Tags = new[] { "MFA" })]
        public async Task<IActionResult> SetupMultiFactorAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);

            await _applicationUserRepository.SetMFAAsync(user, AuthenticationType.Authenticator_Application, null, null);

            user.SetIsResetMFA(true);
            await _userManager.UpdateAsync(user);

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var formattedKey = FormatKey(unformattedKey);
            var authenticatorUri = GenerateQrCodeUri(user.LoginId, unformattedKey);

            var response = new
            {
                formattedkey = formattedKey,
                authenticatorUri = authenticatorUri
            };

            return Ok(response);
        }

        [HttpPost("setup/verify-authenticator")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [SwaggerOperation(OperationId = nameof(VerifyAuthenticator), Tags = new[] { "MFA" })]
        public async Task<IActionResult> VerifyAuthenticator([FromBody] MFAtotpInputDTO model)
        {
            var user = await _userManager.GetUserAsync(User);
            IEnumerable<string> recoveryCode = new List<string>();

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.totp);

            if (!is2faTokenValid)
            {
                return BadRequest("Invalid Code");
            }

            var mfa = await _applicationUserRepository.GetMFAAsync(user);

            var validRecoveryCodeCount = await _userManager.CountRecoveryCodesAsync(user);

            if (validRecoveryCodeCount == 0)
            {
                recoveryCode = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            }

            await _auditLogService.PersistAuditLogAsync(DateTime.UtcNow, "Activate 2FA",
                HttpContext?.Connection?.RemoteIpAddress, _correlationContextAccessor.CorrelationContext.CorrelationId);

            await CompleteSetupMFAAsync(user);

            return Ok(recoveryCode);
        }

        [HttpPost("disable-authenticator")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [SwaggerOperation(OperationId = nameof(DisableAuthenticator), Tags = new[] { "MFA" })]
        public async Task<IActionResult> DisableAuthenticator([FromQuery] string loginId)
        {
            var user = await _userManager.FindByIdAsync(loginId);

            var hasEnabled2FA = await _userManager.GetTwoFactorEnabledAsync(user);

            if (!hasEnabled2FA)
            {
                return BadRequest("Could not disable 2FA for user as it was not enabled.");
            }

            await _applicationUserRepository.SetMFAAsync(user, AuthenticationType.Email, null, null);

            return Ok(true);
        }

        //[HttpPost("remove-authenticator")]
        //[Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        //[SwaggerOperation(OperationId = nameof(RemoveAuthenticator), Tags = new[] { "MFA" })]
        //public async Task<IActionResult> RemoveAuthenticator()
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    var hasEnabled2FA = await _userManager.GetTwoFactorEnabledAsync(user);

        //    if (!hasEnabled2FA)
        //    {
        //        return BadRequest("Could not remove 2FA for user as it was not enabled.");
        //    }

        //    var result = await _userManager.SetTwoFactorEnabledAsync(user, false);

        //    if (!result.Succeeded)
        //    {
        //        return BadRequest("Error occured during disabling 2FA");
        //    }

        //    await _applicationUserRepository.RemoveMFAAsync(user);

        //    return Ok();
        //}

        [HttpPost("remove-authenticator")]
        [Authorize(Policy = AuthenticationPolicies.InternalOnlyPolicy)]
        [SwaggerOperation(OperationId = nameof(RemoveAuthenticatorByLoginId), Tags = new[] { "MFA" })]
        public async Task<IActionResult> RemoveAuthenticatorByLoginId([FromQuery] string loginId)
        {
            var user = await _userManager.FindByIdAsync(loginId);

            var hasEnabled2FA = await _userManager.GetTwoFactorEnabledAsync(user);

            if (!hasEnabled2FA)
            {
                return BadRequest("Could not remove 2FA for user as it was not enabled.");
            }

            var result = await _applicationUserRepository.CustomSetTwoFactorEnabledAsync(user, false);

            if (!result.Succeeded)
            {
                return BadRequest("Error occured during disabling 2FA");
            }

            await _applicationUserRepository.RemoveMFAAsync(user);

            await _auditLogService.PersistAuditLogAsync(DateTime.UtcNow, "Disable 2FA",
                HttpContext?.Connection?.RemoteIpAddress, _correlationContextAccessor.CorrelationContext.CorrelationId);

            return Ok(true);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string loginId, string unformattedKey)
        {
            return string.Format(
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                _urlEncoder.Encode($"[{_env.EnvironmentName}] Tranglo1"),
                _urlEncoder.Encode(loginId),
                unformattedKey
            );
        }

        [HttpPost("request")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [SwaggerOperation(OperationId = nameof(MFAAprovalRequest), Tags = new[] { "MFA" })]
        public async Task<IActionResult> MFAAprovalRequest([FromBody] string code)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var result = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
                if (result)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            return Ok(false);
        }

        [HttpPost("setup/verify-authenticator-successful")]
        [Authorize(Policy = AuthenticationPolicies.InternalOrExternalPolicy)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(OperationId = nameof(VerifyAuthenticator), Tags = new[] { "MFA" })]
        public async Task<IActionResult> VerifyAuthenticatorSuccessful()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
                return BadRequest();

            await CompleteSetupMFAAsync(user);

            return NoContent();
        }

        private async Task CompleteSetupMFAAsync(ApplicationUser user)
        {
            if (user.IsResetMFA ?? false)
            {
                #region Marks user's Reset MFA flow completed
                user.SetIsResetMFA(false);
                await _userManager.UpdateAsync(user);
                #endregion

                await _resetMFASuccessfulEmailSender.NotifyUserAsync(user);
            }
        }
    }
}
