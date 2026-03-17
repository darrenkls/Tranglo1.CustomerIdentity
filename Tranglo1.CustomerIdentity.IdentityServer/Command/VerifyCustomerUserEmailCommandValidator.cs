using FluentValidation;

namespace Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands
{
    public class VerifyCustomerUserEmailCommandValidator : AbstractValidator<VerifyCustomerUserEmailCommand>
    {
        public VerifyCustomerUserEmailCommandValidator()
        {
            RuleFor(v => v.UserId)
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
                .MaximumLength(320).WithMessage("User Id maximum is 320 characters.")
                .NotNull().WithMessage("User Id cannot be null.")
                .NotEmpty().WithMessage("User Id is required.");

            RuleFor(v => v.Token)
                .NotNull().WithMessage("Email confirmation token cannot be NULL.")
                .NotEmpty().WithMessage("Email confirmation token is required.");
        }
    }
}
