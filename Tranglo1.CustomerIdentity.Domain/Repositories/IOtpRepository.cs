using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.OTP;

namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
    public interface IOtpRepository
    {
        public Task<Result<RequisitionOTP>> NewRequisitionOTPAsync(RequisitionOTP requisitionOTP);
        public Task<bool> ValidateOTPAsync(RequisitionOTP requisitionOTP, int userId);
    }
}
