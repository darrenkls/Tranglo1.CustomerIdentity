using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.MFA
{
    public class MFAEmailOTP : Entity
    {
        public string OTP { get; set; }
        public string Email { get; set; }
        public string LoginID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
