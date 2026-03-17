using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Identity;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha;
using static IdentityModel.OidcConstants;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers
{
    public class OwnerPasswordValidatorService<TUser> : IResourceOwnerPasswordValidator
        where TUser : class
    {
        private readonly TrangloSignInManager _signInManager;
        private IEventService _events;
        private readonly TrangloUserManager _userManager;
        private readonly ILogger<OwnerPasswordValidatorService<TUser>> _logger;
        private readonly RecaptchaKey _recaptchaKey;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _ihttpContextAccessor;

        public OwnerPasswordValidatorService(
            TrangloUserManager userManager,
            TrangloSignInManager signInManager,
            IEventService events,
            ILogger<OwnerPasswordValidatorService<TUser>> logger,
            RecaptchaKey recaptchaKey,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _events = events;
            _logger = logger;
            _recaptchaKey = recaptchaKey;
            _httpClientFactory = httpClientFactory;
            _ihttpContextAccessor = httpContextAccessor;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var recaptchaToken = _ihttpContextAccessor.HttpContext?.Request?.Headers["RecaptchaToken"];
            var recaptchaValidate = RecaptchaResponseValidator.Validate(_recaptchaKey.SecretKey, recaptchaToken, _recaptchaKey.RecaptchaScore, _httpClientFactory);

            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(context.UserName);
            }

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, lockoutOnFailure: true);
                if (result.Succeeded && recaptchaValidate.IsSuccess)
                {
                    var sub = await _userManager.GetUserIdAsync(user);

                    _logger.LogInformation("Credentials validated for username: {username}", context.UserName);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(context.UserName, sub, context.UserName, interactive: false));

                    context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                    return;
                }
                else if (recaptchaValidate.IsFailure)
                {
                    _logger.LogInformation("Recaptcha validation failed with the following error: {error}", recaptchaValidate.Error);
                    await _events.RaiseAsync(new UserLoginFailureEvent(context.Request.Raw["recaptchaToken"], "recaptcha validation failed", interactive: false));
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid Recaptcha Token");
                    return;
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: locked out", context.UserName);
                    await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "locked out", interactive: false));

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"Authentication failed for username: {context.UserName}, reason: locked out");
                    return;
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: not allowed", context.UserName);
                    await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "not allowed", interactive: false));

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"Authentication failed for username: {context.UserName}, reason: not allowed");
                    return;
                }
                else
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: invalid credentials", context.UserName);
                    await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid credentials", interactive: false));

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"Authentication failed for username: {context.UserName}, reason: invalid credentials");
                    return;
                }
            }
            else
            {
                _logger.LogInformation("No user found matching username: {username}", context.UserName);
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid username", interactive: false));

                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"No user found matching username: {context.UserName}");
                return;
            }

            //context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username", null);
        }
    }
}
