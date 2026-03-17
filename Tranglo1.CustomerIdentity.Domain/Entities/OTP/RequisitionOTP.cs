using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.OTP
{
    public class RequisitionOTP : Entity
    {
        public string RequisitionCode { get; set; }
        public string OTP { get; set; }
        public string RequestID { get; set; }
        public Guid? RequisitionOTPGroupId { get; set; }
    }
}
