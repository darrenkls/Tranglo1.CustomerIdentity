using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class UserVerificationToken : Entity
    {
        public string Token { get; set; }
        public string Email { get; set; }

    }

}