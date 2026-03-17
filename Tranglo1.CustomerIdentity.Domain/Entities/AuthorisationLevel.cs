using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class AuthorisationLevel : Enumeration
    {
        public AuthorisationLevel() : base()
        {

        }

        public AuthorisationLevel(int id , string name) : base(id, name)
        {

        }

        public static readonly AuthorisationLevel Main = new AuthorisationLevel(1, "Main");
        public static readonly AuthorisationLevel Assistant = new AuthorisationLevel(2, "Assistant");

    }
}
