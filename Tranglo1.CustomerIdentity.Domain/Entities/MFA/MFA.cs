using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.MFA
{
    public class MFA : Entity
    {
        public long? UserId { get; set; }
        public AuthenticationType AuthenticationType { get; set; }
        public string Token { get; set; }
        public string RecoveryCode { get; set; }
    }
}
