using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Meta
{
    public class VerificationIDType : Enumeration
    {
        public VerificationIDType () : base()
        {

        }

        public VerificationIDType(int id, string name) : base(id , name)
        {

        }

        public static readonly VerificationIDType National_ID = new VerificationIDType(1, "Nationality Identity Card");
        public static readonly VerificationIDType Driving_Licence = new VerificationIDType(2, "Driving Licence");
        public static readonly VerificationIDType Passport = new VerificationIDType(3, "Passport");

    }
}
