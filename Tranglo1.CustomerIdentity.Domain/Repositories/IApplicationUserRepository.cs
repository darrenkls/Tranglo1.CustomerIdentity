using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
    public interface IApplicationUserRepository
    {
        Task<List<TrangloStaffAssignment>> GetTrangloStaffAssignmentsByTrangloStaffAsync(TrangloStaff trangloStaff);
        Task<ApplicationUser> GetApplicationUserByLoginId(string loginId);
        Task<ApplicationUser> GetApplicationUserByEmail(Email email);
        Task<CustomerUserRegistration> GetCustomerUserRegistrationsByLoginIdAsync(string loginId);
        Task<CustomerUserRegistration> GetCustomerUserRegistrationsByCompanyNameAsync(string companyName);
        Task<CustomerUserRegistration> GetCustomerUserRegistrationsByCompanyNameAndLoginIdAsync(string companyName, string loginId);
        Task<CustomerUserRegistration> GetCustomerUserRegistrationsByCompanyNameAndLoginIdAndSolutionAsync(string companyName, string loginId, int solution);
        Task<CustomerUserRegistration> UpdateCustomerUserRegistrationsAsync(CustomerUserRegistration customerUserRegistration);
        //Task SaveChanges();
        Task<List<TrangloStaffAssignment>> GetTrangloStaffAssignment(string loginId);
        Task<List<TrangloStaffAssignment>> GetTrangloStaffAssignmentByRole(string roleCode);
        Task<List<TrangloStaffAssignment>> GetTrangloStaffAssignmentByIdAndEntity(string loginId, string entity);
        Task SaveTrangloStaffAssignmentChanges(List<TrangloStaffAssignment> trangloStaffAssignments);
        Task<Result<TrangloStaff>> SaveTrangloUserChanges(TrangloStaff trangloStaff);
        Task AddTrangloUser(TrangloStaff trangloStaff);
        Task AddCustomerUserRegistration(CustomerUserRegistration customerUserRegistration);
        Task<TrangloStaff> GetTrangloUserByLoginId(string loginId);
        Task<TrangloStaffEntityAssignment> GetTrangloStaffEntityAssignment(string loginId, string trangloEntity);
        Task<List<TrangloStaffEntityAssignment>> GetTrangloStaffEntityAssignmentById(string loginId);
        Task<TrangloEntity> GetTrangloEntityByCodeAsync(string trangloEntityCode);
        Task UpdateTrangloStaffEntityAssignment(TrangloStaffEntityAssignment trangloStaffEntityAssignment);
        // Task<TrangloStaffAssignment> AddApplicationUser(ApplicationUser applicationUser);
        Task<Result<ApplicationUser>> UpdateApplicationUser(ApplicationUser applicationUser, CancellationToken cancellationToken);
        Task<List<TrangloStaffEntityAssignment>> GetTrangloStaffEntityAssignmentByUserId(int userId);
        Task<ApplicationUser> GetTrangloUserByUserId(int userId);
        Task<CustomerUser> GetCustomerUserAsync(string loginId);
        Task<ApplicationUser> GetApplicationUserByUserId(long userId);

        //MFA
        Task<Result<MFAEmailOTP>> NewMFAEMailOTPAsync(MFAEmailOTP mfaEmailOTP);
        Task ReplaceCodesAsync(ApplicationUser user, IEnumerable<string> recoveryCodes);
        Task SetRecoveryCodeAsync(ApplicationUser user, string recoveryCode);
        Task<MFA> GetMFAAsync(ApplicationUser user);
        Task<int> CountValidRecoveryCode(ApplicationUser user);
        Task SetMFAAsync(ApplicationUser user, AuthenticationType authenticationType, string? token, string? recoveryCode);
        Task RemoveMFAAsync(ApplicationUser user);
        Task<IdentityResult> CustomSetTwoFactorEnabledAsync(ApplicationUser user, bool enabled);
        Task<IdentityResult> CustomSetTwoFactorDisabledAsync(ApplicationUser user, bool disabled);
        Task<MFAEmailOTP> GetMFAEmailOTPByOTPAsync(string otpCode);

        Task<MFAEmailOTP> GetMFAEmailOTPAsync(string loginId);
        Task<CompanyUserBlockStatus> GetCompanyUserBlockStatusAsync(CompanyUserBlockStatus status);

        Task<MFAResetRequest> GetMFAResetRequestByTokenAsync(string token);
        Task<MFAResetRequest> InsertMFAResetRequestAsync(MFAResetRequest mfaResetRequest);
        Task<MFAResetRequest> UpdateMFAResetRequestAsync(MFAResetRequest mfaResetRequest);
        Task<IdentityResult> SetIsResetMFAAsync(ApplicationUser applicationUser, bool isMFAReset);
    }
}
