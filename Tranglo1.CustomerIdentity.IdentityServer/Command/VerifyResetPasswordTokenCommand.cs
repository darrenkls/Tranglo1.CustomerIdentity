using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;

namespace Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands
{
    public class VerifyResetPasswordTokenCommand : IRequest<IdentityResult>
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string NewPassword { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string ConfirmPassword { get; set; }

        public class VerifyResetPasswordTokenCommandHander : IRequestHandler<VerifyResetPasswordTokenCommand, IdentityResult>
        {
            private readonly TrangloUserManager _userManager;
            public VerifyResetPasswordTokenCommandHander(TrangloUserManager userManager)
            {
                _userManager = userManager;
            }

            public async Task<IdentityResult> Handle(VerifyResetPasswordTokenCommand request, 
                CancellationToken cancellationToken)
            {
                //Use FindById. FindByEmail may return more than one records if the email is a Tranglo Staff and used to 
                //test as a customer user.
                var user = await _userManager.FindByIdAsync(request.Email);

				if (user == null)
				{
                    return IdentityResult.Failed(new IdentityError()
                    {
                        Description = $"Email: {request.Email} is not a valid customer user email."
                    });
				}

                return await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            }
        }
    }
}
