using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class InviteePasswordVerificationCommand : IRequest<IdentityResult>
    {
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string ResetPasswordToken { get; set; }
        public string EmailConfirmationToken { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string NewPassword { get; set; }
    }

    public class InviteePasswordVerificationCommandHandler : IRequestHandler<InviteePasswordVerificationCommand, IdentityResult>
    {
        private readonly ILogger<InviteePasswordVerificationCommandHandler> _logger;
        private readonly TrangloUserManager _userManager;

        public InviteePasswordVerificationCommandHandler(
            TrangloUserManager userManager,
            ILogger<InviteePasswordVerificationCommandHandler> logger
            )
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityResult> Handle(InviteePasswordVerificationCommand request, CancellationToken cancellationToken)
        {
            CustomerUser _User = await _userManager.FindByIdAsync(request.LoginId) as CustomerUser;

            if (_User == null)
            {
                _logger.LogError("FindByIdAsync", $"Email [{request.LoginId}] does not exist.");
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = $"Email [{request.LoginId}] does not exist."
                    });
            }

            #region Reset Password
            string resetTokenString = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetPasswordToken));
            if (_User != null && _User is CustomerUser)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(
                                                            _User,
                                                            resetTokenString,
                                                            request.NewPassword
                                                          );

                if (!result.Succeeded)
                {
                    _logger.LogError("ResetPasswordAsync", result.Errors);
                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = string.Join(",", result.Errors.Select(x => x.Description).ToList())
                        });
                }
            }
            else
            {
                return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = $"Email: [{request.LoginId}] is not a valid customer user email."
                        });
            }
            #endregion

            #region Email Confirmation
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(_User);
            IdentityResult ConfirmEmailResult = await _userManager.ConfirmInviteeEmailAsync(
                                                            _User,
                                                            emailConfirmationToken,
                                                            cancellationToken
                                                        );
            if (!ConfirmEmailResult.Succeeded)
            {
                _logger.LogError("ConfirmEmailAsync", $"Email confirmation for [{request.LoginId}] failed.");
                return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = $"Email confirmation for [{request.LoginId}] failed."
                        });
            }
            #endregion

            return IdentityResult.Success;
        }
    }
}
