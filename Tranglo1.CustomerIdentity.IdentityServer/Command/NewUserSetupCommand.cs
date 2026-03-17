using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class NewUserSetupCommand : IRequest<IdentityResult>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }

    public class NewUserSetupCommandHandler : IRequestHandler<NewUserSetupCommand, IdentityResult>
    {
        private readonly ILogger<VerifyCustomerUserEmailCommandHandler> _logger;
        private readonly TrangloUserManager _userManager;

        public NewUserSetupCommandHandler(
            TrangloUserManager userManager,
            ILogger<VerifyCustomerUserEmailCommandHandler> logger
            )
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityResult> Handle(NewUserSetupCommand request, CancellationToken cancellationToken)
        {
            CustomerUser _User = await _userManager.FindByEmailAsync(request.UserId) as CustomerUser;
            
            if (_User == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = $"Email {request.UserId} does not exist."
                    });
            }

            var tokenString = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            if (!await _userManager.VerifyUserTokenAsync(_User,
                _userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", tokenString))
            {
                return IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            //var ConfirmEmailResult = await _userManager.ConfirmEmailAsync(_User, request.Token);
            //if (!ConfirmEmailResult.Succeeded)
            //{
            //    return IdentityResult.Failed(
            //            new IdentityError
            //            {
            //                Description = $"Email confirmation for {request.UserId} failed."
            //            });
            //}

            return IdentityResult.Success;
        }
    }
}
