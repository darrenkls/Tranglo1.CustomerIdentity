using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands
{
    public class VerifyCustomerUserResetPasswordCommandValidator : AbstractValidator<VerifyResetPasswordTokenCommand>
    {
        public VerifyCustomerUserResetPasswordCommandValidator()
        {
            RuleFor(v => v.NewPassword)
                .Equal(v => v.ConfirmPassword).WithMessage("New Password and Confirm Password do not match.");
        }
    }
}
