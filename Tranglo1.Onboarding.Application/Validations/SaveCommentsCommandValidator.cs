using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Command;

namespace Tranglo1.CustomerIdentity.IdentityServer.Validations
{    
    public class SaveCommentsCommandValidator : AbstractValidator<SaveCommentsCommand>
    {
        public SaveCommentsCommandValidator()
        {
            RuleFor(v => v.Comment)
                .MaximumLength(150).WithMessage("Comment maximum length is 150 characters.")
                .NotEmpty().WithMessage("Comment is required.");
        }
    }
    
}
