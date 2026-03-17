using CSharpFunctionalExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.OTP;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Dapper;
using System.Linq;

namespace Tranglo1.CustomerIdentity.Infrastructure.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly OTPDbContext dbContext;

        private class RequisitionOTPWithDate : RequisitionOTP
        {
            public DateTime CreatedDate { get; set; }
        }

        public OtpRepository(OTPDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }
        
        public async Task<Result<RequisitionOTP>> NewRequisitionOTPAsync(RequisitionOTP requisitionOTP)
        {
            dbContext.RequisitionOTPs.Add(requisitionOTP);
            await dbContext.SaveChangesAsync();
            return requisitionOTP;
        }

        public async Task<bool> ValidateOTPAsync(RequisitionOTP requisitionOTP, int userId)
        {
            using (var connection = new SqlConnection(dbContext.Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                var result = (await connection.QueryAsync<RequisitionOTPWithDate>(
                   "GetLatestUserOTP",
                   new
                   {
                       @RequisitionCode = requisitionOTP.RequisitionCode,
                       @CreatedBy = userId
                   },
                   null, null, CommandType.StoredProcedure)).ToList();

                if(result.Count == 0)
                {
                    return false;
                }
                var OTPResult = result.First();
                if(OTPResult.RequestID == requisitionOTP.RequestID && OTPResult.OTP == requisitionOTP.OTP && DateTime.UtcNow < OTPResult.CreatedDate.AddMinutes(5))
                {
                    return true;
                }
                return false;
            }
            
        }
    }
}
