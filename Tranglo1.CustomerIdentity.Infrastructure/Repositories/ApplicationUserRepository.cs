using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationUserDbContext dbContext;

        public ApplicationUserRepository(ApplicationUserDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<TrangloStaffAssignment>> GetTrangloStaffAssignmentsByTrangloStaffAsync(TrangloStaff trangloStaff)
        {
            if (trangloStaff != null)
            {
                var query = dbContext.TrangloStaffAssignments.Where(x => x.LoginId == trangloStaff.LoginId).ToListAsync();
                return await query;
            }

            return await Task.FromResult<List<TrangloStaffAssignment>>(null);
        }

        public async Task<ApplicationUser> GetApplicationUserByLoginId(string loginId)
        {
            if (loginId != null)
            {
                // var query = dbContext.ApplicationUsers.Where(x => x.LoginId == loginId);
                return await dbContext.ApplicationUsers.Include(x => x.AccountStatus).FirstOrDefaultAsync(x => x.LoginId == loginId);
            }

            return await Task.FromResult<ApplicationUser>(null);
        }
        public async Task<ApplicationUser> GetApplicationUserByEmail(Email email)
        {
            if (email != null)
            {
                var query = dbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == email);
                return await query;
            }

            return await Task.FromResult<ApplicationUser>(null);
        }

        public async Task<CustomerUserRegistration> GetCustomerUserRegistrationsByLoginIdAsync(string loginId)
        {
            if (loginId != null)
            {
                var query = dbContext.CustomerUserRegistrations
                    .Include(x => x.PartnerRegistrationLeadsOrigin)
                    .FirstOrDefaultAsync(x => x.LoginId == loginId);
                return await query;
            }

            return await Task.FromResult<CustomerUserRegistration>(null);
        }

        public async Task<CustomerUserRegistration> GetCustomerUserRegistrationsByCompanyNameAsync(string companyName)
        {
            var query = dbContext.CustomerUserRegistrations.FirstOrDefaultAsync(x => x.CompanyName == companyName);
            return await query;
        }

        public async Task<CustomerUserRegistration> GetCustomerUserRegistrationsByCompanyNameAndLoginIdAsync(string companyName, string loginId)
        {
            var query = dbContext.CustomerUserRegistrations.FirstOrDefaultAsync(x => x.CompanyName == companyName && x.LoginId == loginId);
            return await query;
        }
        public async Task<CustomerUserRegistration> GetCustomerUserRegistrationsByCompanyNameAndLoginIdAndSolutionAsync(string companyName, string loginId, int solution)
        {
            var query = dbContext.CustomerUserRegistrations.FirstOrDefaultAsync(x => x.CompanyName == companyName && x.LoginId == loginId && x.SolutionCode == solution);
            return await query;
        }

        public async Task<List<TrangloStaffAssignment>> GetTrangloStaffAssignment(string loginId)
        {
            if (loginId != null)
            {
                var query = dbContext.TrangloStaffAssignments.Where(x => x.LoginId == loginId).ToListAsync();
                return await query;
            }

            return await Task.FromResult<List<TrangloStaffAssignment>>(null);
        }
        public async Task<List<TrangloStaffAssignment>> GetTrangloStaffAssignmentByRole(string roleCode)
        {
            var query = dbContext.TrangloStaffAssignments.Where(x => x.RoleCode == roleCode).ToListAsync();
            return await query;
        }

        public async Task<List<TrangloStaffAssignment>> GetTrangloStaffAssignmentByIdAndEntity(string loginId, string entity)
        {
            if (loginId != null)
            {
                var query = dbContext.TrangloStaffAssignments.Where(x => x.LoginId == loginId && x.TrangloEntity == entity).ToListAsync();
                return await query;
            }

            return await Task.FromResult<List<TrangloStaffAssignment>>(null);
        }

        //public async Task<ApplicationUser> AddApplicationUser(ApplicationUser applicationUser)
        //{
        //    this.dbContext.Add(applicationUser);    
        //    await this.dbContext.SaveChangesAsync();

        //    return applicationUser;
        //}

        public async Task<Result<TrangloStaff>> SaveTrangloUserChanges(TrangloStaff trangloStaff)
        {
            this.dbContext.Update(trangloStaff);
            await this.dbContext.SaveChangesAsync();

            return trangloStaff;
        }
        public async Task SaveTrangloStaffAssignmentChanges(List<TrangloStaffAssignment> trangloStaffAssignments)
        {
            this.dbContext.AttachRange(trangloStaffAssignments);
            await this.dbContext.SaveChangesAsync();
        }
        public async Task UpdateTrangloStaffEntityAssignment(TrangloStaffEntityAssignment trangloStaffEntityAssignment)
        {
            this.dbContext.AttachRange(trangloStaffEntityAssignment);
            await this.dbContext.SaveChangesAsync();

        }
        public async Task AddTrangloUser(TrangloStaff trangloStaff)
        {
            this.dbContext.Add(trangloStaff);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task AddCustomerUserRegistration(CustomerUserRegistration customerUserRegistration)
        {
            this.dbContext.Add(customerUserRegistration);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<TrangloStaff> GetTrangloUserByLoginId(string loginId)
        {
            var query = dbContext.TrangloStaffs.Include(x => x.TrangloStaffAssignments).Include(x => x.TrangloStaffEntityAssignments).FirstOrDefaultAsync(x => x.LoginId == loginId);
            return await query;
        }
        public async Task<TrangloStaffEntityAssignment> GetTrangloStaffEntityAssignment(string loginId, string trangloEntity)
        {
            var query = dbContext.TrangloStaffEntityAssignment.Where(x => x.LoginId == loginId && x.TrangloEntity == trangloEntity).FirstOrDefaultAsync();
            //return await dbContext.TrangloStaffEntityAssignment.FirstOrDefault(x => x.LoginId == loginId && x.TrangloEntity == trangloEntity);
            return await query;
        }

        public async Task<List<TrangloStaffEntityAssignment>> GetTrangloStaffEntityAssignmentById(string loginId)
        {
            var query = dbContext.TrangloStaffEntityAssignment.Where(x => x.LoginId == loginId).Include(x => x.AccountStatus).Include(x => x.BlockStatus).ToListAsync();
            //return await dbContext.TrangloStaffEntityAssignment.FirstOrDefault(x => x.LoginId == loginId && x.TrangloEntity == trangloEntity);
            return await query;
        }

        public async Task<TrangloEntity> GetTrangloEntityByCodeAsync(string trangloEntityCode)
        {
            var query = dbContext.TrangloEntity.Where(x => x.TrangloEntityCode == trangloEntityCode).FirstOrDefaultAsync();
            return await query;
        }

        public async Task<List<TrangloStaffEntityAssignment>> GetTrangloStaffEntityAssignmentByUserId(int userId)
        {
            var query = dbContext.TrangloStaffEntityAssignment.Where(x => x.UserId == userId).ToListAsync();
            //return await dbContext.TrangloStaffEntityAssignment.FirstOrDefault(x => x.LoginId == loginId && x.TrangloEntity == trangloEntity);
            return await query;
        }

        public async Task<ApplicationUser> GetTrangloUserByUserId(int userId)
        {
            var query = dbContext.ApplicationUsers.Where(x => x.Id == userId).FirstOrDefaultAsync();
            return await query;
        }
        public async Task<Result<ApplicationUser>> UpdateApplicationUser(ApplicationUser applicationUser, CancellationToken cancellationToken)
        {
            this.dbContext.Update(applicationUser);
            await this.dbContext.SaveChangesAsync();

            return applicationUser;
        }

        public async Task<CustomerUserRegistration> UpdateCustomerUserRegistrationsAsync(CustomerUserRegistration customerUserRegistration)
        {
            this.dbContext.Update(customerUserRegistration);
            await this.dbContext.SaveChangesAsync();

            return customerUserRegistration;
        }

        public async Task<CustomerUser> GetCustomerUserAsync(string loginId)
        {
            var query = await this.dbContext.CustomerUsers.Where(x => x.LoginId == loginId)
                .FirstOrDefaultAsync();
            return query;
        }
   

        public async Task<ApplicationUser> GetApplicationUserByUserId(long userId)
        {
            if (userId != 0)
            {
                return await dbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);
            }

            return await Task.FromResult<ApplicationUser>(null);
        }

        //MFA
        public async Task<Result<MFAEmailOTP>> NewMFAEMailOTPAsync(MFAEmailOTP mfaEmailOTP)
        {
            dbContext.MFAEmailOTPs.Add(mfaEmailOTP);
            await dbContext.SaveChangesAsync();
            return mfaEmailOTP;
        }

        public async Task ReplaceCodesAsync(ApplicationUser user, IEnumerable<string> recoveryCodes)
        {
            var codes = string.Join(";", recoveryCodes);
            await SetRecoveryCodeAsync(user, codes);
        }

        public async Task SetRecoveryCodeAsync(ApplicationUser user, string recoveryCode)
        {
            var result = await GetMFAAsync(user);

            if (result != null)
            {
                result.RecoveryCode = recoveryCode;

                dbContext.MFA.Update(result);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<MFA> GetMFAAsync(ApplicationUser user)
        {
            var entry = await this.dbContext.MFA
                .Where(t => t.UserId == user.Id).FirstOrDefaultAsync();

            return await Task.FromResult(entry);
        }

        public async Task<int> CountValidRecoveryCode(ApplicationUser user)
        {
            var entry = await this.dbContext.MFA
                .Where(t => t.UserId == user.Id).FirstOrDefaultAsync();

            return entry.RecoveryCode?.Split(";")?.Length ?? 0;
        }

        public async Task SetMFAAsync(ApplicationUser user, AuthenticationType authenticationType, string? token, string? recoveryCode)
        {
            var result = await GetMFAAsync(user);

            if (result != null)
            {
                result.AuthenticationType = authenticationType;
                result.Token = token;
                result.RecoveryCode = recoveryCode;
                dbContext.MFA.Update(result);
            }
            else
            {
                var mfa = new MFA();

                mfa.AuthenticationType = authenticationType;
                mfa.UserId = user.Id;
                mfa.Token = token;
                mfa.RecoveryCode = recoveryCode;
                dbContext.MFA.Add(mfa);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveMFAAsync(ApplicationUser user)
        {
            var mfa = await GetMFAAsync(user);

            if (mfa != null)
            {
                dbContext.MFA.Remove(mfa);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<IdentityResult> CustomSetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            var entry = await dbContext.ApplicationUsers.Where(u => u.Id == user.Id).FirstOrDefaultAsync();

            if (entry == null) 
            {
                return IdentityResult.Failed();
            }

            entry.SetTwoFactorEnabled(enabled);
            dbContext.ApplicationUsers.Update(entry);

            await dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> CustomSetTwoFactorDisabledAsync(ApplicationUser user, bool disabled)
        {
            var entry = await dbContext.ApplicationUsers.Where(u => u.Id == user.Id).FirstOrDefaultAsync();

            if (entry == null)
            {
                return IdentityResult.Failed();
            }

            entry.SetTwoFactorEnabled(disabled);
            dbContext.ApplicationUsers.Update(entry);

            await dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<MFAEmailOTP> GetMFAEmailOTPByOTPAsync(string otpCode)
        {
           var entry = await dbContext.MFAEmailOTPs
                .Where(x => x.OTP == otpCode).FirstOrDefaultAsync();

            return entry;
        }

        public async Task<MFAEmailOTP> GetMFAEmailOTPAsync(string loginId)
        {
            var entry = await dbContext.MFAEmailOTPs
                 .Where(x => x.LoginID == loginId)
                 .OrderByDescending(x => x.CreatedDate)
                 .FirstOrDefaultAsync();

            return entry;
        }

		public async Task<CompanyUserBlockStatus> GetCompanyUserBlockStatusAsync(CompanyUserBlockStatus status)
		{
            var userBlockStatus = dbContext.CompanyUserBlockStatus.Local.FirstOrDefault(x => x.Id == status.Id);

			if (userBlockStatus == null)
				userBlockStatus = await dbContext.CompanyUserBlockStatus.FirstOrDefaultAsync(x => x.Id == status.Id);

            return userBlockStatus;
		}

        public async Task<MFAResetRequest> GetMFAResetRequestByTokenAsync(string token)
        {
            return await dbContext.MFAResetRequests
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task<MFAResetRequest> InsertMFAResetRequestAsync(MFAResetRequest mfaResetRequest)
        {
            dbContext.Add(mfaResetRequest);
            await dbContext.SaveChangesAsync();

            return mfaResetRequest;
        }

        public async Task<MFAResetRequest> UpdateMFAResetRequestAsync(MFAResetRequest mfaResetRequest)
        {
            dbContext.Attach(mfaResetRequest);
            await dbContext.SaveChangesAsync();

            return mfaResetRequest;
        }

        public async Task<IdentityResult> SetIsResetMFAAsync(ApplicationUser applicationUser, bool isResetMFA)
        {
            var entry = await dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == applicationUser.Id);

            if (entry == null)
            {
                return IdentityResult.Failed();
            }

            entry.SetIsResetMFA(isResetMFA);

            await dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }
    }
}