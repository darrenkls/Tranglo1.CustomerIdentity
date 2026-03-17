using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.MFA
{
    public class AuthenticationType : Enumeration
    {
        public AuthenticationType() : base()
        {

        }

        public AuthenticationType(int id, string name)
            : base(id, name)
        {

        }

        public static readonly AuthenticationType Email = new AuthenticationType(1, "Email");
        public static readonly AuthenticationType Authenticator_Application = new AuthenticationType(2, "Authenticator Application");
    }
}
