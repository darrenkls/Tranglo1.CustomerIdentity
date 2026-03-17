using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.Domain.Entities.Specifications.BusinessProfiles;
using Tranglo1.CustomerIdentity.Domain.Entities.Specifications.CustomerUserBusinessProfileRoles;
using Tranglo1.CustomerIdentity.Domain.Entities.Specifications.CustomerUserBusinessProfiles;
using Tranglo1.CustomerIdentity.Domain.Repositories;

namespace Tranglo1.CustomerIdentity.Domain.DomainServices
{
    public class TrangloUserManager : UserManager<ApplicationUser>
    {
        public const string Domain = "TRANGLO";
        public const string DomainComponent = "NET";
        private readonly ILogger<UserManager<ApplicationUser>> _logger;
        private readonly IBusinessProfileRepository businessProfileRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly BusinessProfileService _businessProfileService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly ITrangloRoleRepository _trangloRoleRepository;
        private readonly IExternalUserRoleRepository _externalUserRoleRepository;

        protected IBusinessProfileRepository Repository => businessProfileRepository;

        public TrangloUserManager(IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger,
            IBusinessProfileRepository businessProfileRepository,
            IInvitationRepository invitationRepository,
            BusinessProfileService businessProfileService,
            IPartnerRepository partnerRepository,
            ITrangloRoleRepository trangloRoleRepository,
            IApplicationUserRepository applicationUserRepository,
            IExternalUserRoleRepository externalUserRoleRepository)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _logger = logger;
            this.businessProfileRepository = businessProfileRepository;
            _invitationRepository = invitationRepository;
            _businessProfileService = businessProfileService;
            _trangloRoleRepository = trangloRoleRepository;
            _applicationUserRepository = applicationUserRepository;
            _partnerRepository = partnerRepository;
            _externalUserRoleRepository = externalUserRoleRepository;
        }

        public override async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            if (email.EndsWith("tranglo.net"))
            {
                throw new NotImplementedException();
            }

            return await base.FindByEmailAsync(email);
        }

        public override Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            return base.FindByLoginAsync(loginProvider, providerKey);
        }


        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            //if(user is TrangloStaff)
            //{
            // return IdentityResult.Failed(new IdentityError()
            // {
            //Code = "006",
            // Description = "Cannot create AD user."
            //});
            //}

            return await base.CreateAsync(user);
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await base.CreateAsync(user, password);
        }

        public override Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            return base.DeleteAsync(user);
        }

        public override async Task<IdentityResult> SetEmailAsync(ApplicationUser user, string email)
        {
            return user is TrangloStaff
                ? IdentityResult.Failed(new IdentityError()
                {
                    Code = "004",
                    Description = "Email must be set through AD."
                })
                : await base.SetEmailAsync(user, email);
        }

        public override async Task<IdentityResult> ChangeEmailAsync(ApplicationUser user, string newEmail, string token)
        {
            return user is TrangloStaff
                ? IdentityResult.Failed(new IdentityError()
                {
                    Code = "004",
                    Description = "Email must be changed through AD."
                })
                : await base.ChangeEmailAsync(user, newEmail, token);
        }

        public override async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return user is TrangloStaff
                ? await Task.FromResult<string>(null) :
                await base.GenerateUserTokenAsync(user, Options.Tokens.EmailConfirmationTokenProvider, ConfirmEmailTokenPurpose);
        }

        public override async Task<IdentityResult> SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            return user is TrangloStaff
                ? IdentityResult.Failed(new IdentityError()
                {
                    Code = "006",
                    Description = "Phone number must be set through AD."
                })
                : await base.SetPhoneNumberAsync(user, phoneNumber);
        }

        public override async Task<string> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber)
        {
            return user is TrangloStaff ? await Task.FromResult<string>(null) : await base.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        public override async Task<bool> VerifyChangePhoneNumberTokenAsync(ApplicationUser user, string token, string phoneNumber)
        {
            return user is TrangloStaff ? await Task.FromResult(false) : await base.VerifyChangePhoneNumberTokenAsync(user, token, phoneNumber);
        }

        public override async Task<IdentityResult> ChangePhoneNumberAsync(ApplicationUser user, string phoneNumber, string token)
        {
            if (user is TrangloStaff)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "006",
                    Description = "Cannot update AD user."
                });
            }

            return await base.ChangePhoneNumberAsync(user, phoneNumber, token);
        }

        public override async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            if (user is TrangloStaff)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "005",
                    Description = "Cannot update AD user."
                });
            }

            return await base.UpdateAsync(user);
        }

        public override Task UpdateNormalizedEmailAsync(ApplicationUser user)
        {
            if (user is TrangloStaff)
            {
                return Task.CompletedTask;
            }

            return base.UpdateNormalizedEmailAsync(user);
        }

        public override async Task<IdentityResult> SetUserNameAsync(ApplicationUser user, string userName)
        {
            return user is TrangloStaff
                ? IdentityResult.Failed(new IdentityError()
                {
                    Code = "005",
                    Description = "User name must be set through AD."
                })
                : await base.SetUserNameAsync(user, userName);
        }

        public override Task UpdateNormalizedUserNameAsync(ApplicationUser user)
        {
            if (user is TrangloStaff)
            {
                return Task.CompletedTask;
            }

            return base.UpdateNormalizedUserNameAsync(user);
        }

        protected override async Task<IdentityResult> UpdatePasswordHash(ApplicationUser user, string newPassword, bool validatePassword)
        {
            if (user is TrangloStaff)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "004",
                    Description = "Password must be changed through AD."
                });
            }

            return await base.UpdatePasswordHash(user, newPassword, validatePassword);
        }

        /*
        protected override async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)

            if (user is TrangloStaff)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "003",
                    Description = "Updating Tranglo user is not supported."
                });
            }
            return await base.UpdateUserAsync(user);
    }
        */

        public override Task<IdentityResult> UpdateSecurityStampAsync(ApplicationUser user)
        {
            return base.UpdateSecurityStampAsync(user);
        }

        public override async Task<IdentityResult> AddPasswordAsync(ApplicationUser user, string password)
        {
            if (user is TrangloStaff)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "001", Description = "Create password for Tranglo user is not supported." });
            }

            return await base.AddPasswordAsync(user, password);
        }

        public override async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            if (user is TrangloStaff)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "002", Description = "Tranglo user must change user via AD." });
            }

            return await base.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public override async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            return user is TrangloStaff
                ? IdentityResult.Failed(new IdentityError() { Code = "001", Description = "Reset password for Tranglo user is not supported." })
                : await base.ResetPasswordAsync(user, token, newPassword);
        }

        public override async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return user is TrangloStaff ? await Task.FromResult<string>(null) : await base.GeneratePasswordResetTokenAsync(user);
        }

        public override async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var _AdditionalClaims = base.GetClaimsAsync(user);
            List<Claim> _Claims = new List<Claim>();

            if (user is TrangloStaff trangloStaff)
            {
                _Claims.AddRange(await GetFixedClaimsForTrangloStaff(trangloStaff));
            }
            else if (user is CustomerUser customerUser)
            {
                _Claims.AddRange(await GetFixedClaimsForCustomerUserAsync(customerUser));
            }

            //Add current time ( Will use the claims for file generation )
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            _Claims.Add(new Claim("login_time", timestamp.ToString()));

            _Claims.AddRange(await _AdditionalClaims);

            var mfa = await _applicationUserRepository.GetMFAAsync(user);
            bool isNewUser = mfa is null && !user.TwoFactorEnabled;
            if (isNewUser)
            {
                _Claims.Add(new Claim(ClaimCode.IS_NEW_USER, true.ToString()));
            }

            return _Claims;
        }

        private async Task<List<Claim>> GetFixedClaimsForTrangloStaff(TrangloStaff trangloStaff)
        {
            List<Claim> _Claims = new List<Claim>();

            _Claims.Add(new Claim(ClaimTypes.NameIdentifier, trangloStaff.LoginId));
            _Claims.Add(new Claim(ClaimTypes.Email, trangloStaff.Email.Value));
            _Claims.Add(new Claim(ClaimTypes.Name, trangloStaff.FullName.Value));

            // UserId
            _Claims.Add(new Claim("userid", trangloStaff.Id.ToString()));

            //Every AD user confirm will have this claim. Admin portal will 
            //only allow user with this claim to access it.
            _Claims.Add(new Claim(ClaimTypes.Role, "InternalUser"));

            //some other additional info that can get from AD
            //_Claims.Add(new Claim("department", "Tech"));
            _Claims.Add(new Claim("type", "internal"));

            #region Add company, dept & user role
            /*
             * There are possibility that tranglo staff has multiple companies,departments,roles
             * Format as follow:
             * CompanyFormat: company.<EntityCode>: <CompanyDescription>
             * DepartmentFormat: company.<EntityCode>.dept.<DeptCode>: <DepartmentDescription>
             * RoleFormat: company.<EntityCode>.dept.<DeptCode>.role.<roleCode>: <RoleDescription>
             * Sample claim values:
                company.tsb: "Tranglo Sdn Bhd",
                company.tsb.dept.1 : "Technology"
                company.tsb.dept.2 : "finance"
                company.tsb.dept.1.role.1 : "Software Developer"
                company.tsb.dept.2.role.2 : "HOD"
                company.tel: "Tranglo Pte Ltd",
                company.tel.dept.5 : "Technology"
                company.tel.dept.8.role.5 : "Senior Software Developer"
                company.tel.dept.5.role_code : "TECH01,TECH02,TECH03"
                company.tel.dept.8.role_code : "PRODUCT01,PRODUCT02"
            */
            List<string> trangloEntities = new List<string>();

            var trangloStaffEntityAssignments = await _applicationUserRepository.GetTrangloStaffEntityAssignmentById(trangloStaff.LoginId);
            foreach (var trangloStaffEntityAssignment in trangloStaffEntityAssignments)
            {
                string trangloCompanyClaim = "";
                string trangloEntityClaim = "";
                StringBuilder roleCodes = new StringBuilder();
                if (trangloStaffEntityAssignment != null)
                {
                    var trangloStaffAssignments = await _applicationUserRepository.GetTrangloStaffAssignmentByIdAndEntity(trangloStaff.LoginId, trangloStaffEntityAssignment.TrangloEntity);
                    trangloCompanyClaim = $"entity.{trangloStaffEntityAssignment.TrangloEntity}";
                    trangloEntityClaim = $"entity.{trangloStaffEntityAssignment}";
                    var entityName = TrangloEntity.GetByEntityByTrangloId(trangloStaffEntityAssignment.TrangloEntity);
                    var existTrangloCompanyClaim = _Claims.Exists(x => x.Type == trangloCompanyClaim);
                    if (!existTrangloCompanyClaim)
                    {
                        _Claims.Add(new Claim(trangloCompanyClaim, entityName.Name, ClaimValueTypes.String));

                        foreach (var trangloStaffAssignment in trangloStaffAssignments)
                        {
                            TrangloDepartment deptId = Enumeration.FindById<TrangloDepartment>(trangloStaffAssignment.TrangloDepartmentCode);
                            var getRole = await _trangloRoleRepository.GetTrangloRoleByCodeAsync(trangloStaffAssignment.RoleCode);

                            if (deptId != null)
                            {
                                _Claims.Add(new Claim(trangloCompanyClaim + $".dept.{trangloStaffAssignment.TrangloDepartmentCode}", deptId.Name, ClaimValueTypes.String));
                            }
                            if (getRole != null)
                            {
                                if (trangloStaffAssignment != trangloStaffAssignments.Last())
                                {
                                    roleCodes.Append($"{trangloStaffAssignment.RoleCode},");
                                }
                                else
                                {
                                    roleCodes.Append(trangloStaffAssignment.RoleCode);
                                }

                                _Claims.Add(new Claim(trangloCompanyClaim + $".dept.{trangloStaffAssignment.TrangloDepartmentCode}.role.{trangloStaffAssignment.RoleCode}", getRole.Description, ClaimValueTypes.String));
                            }
                            _Claims.Add(new Claim($"blck_stat.{trangloStaffEntityAssignment.TrangloEntity}", trangloStaffEntityAssignment.BlockStatus.Id.ToString(), ClaimValueTypes.String));
                        }
                    }
                }


                _Claims.Add(new Claim($"{trangloCompanyClaim}.role_code.", roleCodes.ToString(), ClaimValueTypes.String));


            }

            trangloEntities = await _trangloRoleRepository.GetTrangloEntityByLoginId(trangloStaff.LoginId); ;
            var trangloEntityList = trangloEntities.Distinct();

            var trangloEntityString = String.Join(",", trangloEntityList);
            _Claims.Add(new Claim($"entity_codes", trangloEntityString.ToString(), ClaimValueTypes.String));

            #endregion
            return _Claims;
        }

        private class SubscriptionAccountStatus
        {
            public long PartnerSubscriptionCode { get; set; }
            public long? PartnerAccountStatusType { get; set; }
        }

        private async Task<List<Claim>> GetFixedClaimsForCustomerUserAsync(CustomerUser customerUser)
        {
            List<Claim> _Claims = new List<Claim>();

            _Claims.Add(new Claim(ClaimTypes.NameIdentifier, customerUser.LoginId));
            _Claims.Add(new Claim(ClaimTypes.Email, customerUser.Email.Value));
            _Claims.Add(new Claim(ClaimTypes.Name, customerUser.FullName.Value));

            //TODELETE
            _Claims.Add(new Claim("userid", customerUser.Id.ToString()));

            _Claims.Add(new Claim("type", "external"));

            #region Add company name & user role
            var customerUserBusinessProfileSpec = Specification<CustomerUserBusinessProfile>.All;
            var byUserID = new CustomerUserBusinessProfileByUserID(customerUser.Id);
            customerUserBusinessProfileSpec = customerUserBusinessProfileSpec.And(byUserID);

            var customerUserBusinessProfileList = await Repository.GetCustomerUserBusinessProfilesAsync(customerUserBusinessProfileSpec);

            foreach (var profile in customerUserBusinessProfileList)
            {
                var customerUserBusinessProfileCode = profile.Id;
                var businessProfileCode = profile.BusinessProfileCode;

                var businessProfilesSpec = Specification<BusinessProfile>.All;
                var companyNameByBusinessProfileCode = new CompanyNameByBusinessProfileCode(businessProfileCode);
                businessProfilesSpec = businessProfilesSpec.And(companyNameByBusinessProfileCode);
                var businessProfileList = await Repository.GetBusinessProfilesAsync(businessProfilesSpec);
                var companyId = businessProfileList.FirstOrDefault().Id;
                var companyName = businessProfileList.FirstOrDefault().CompanyName;
                var accountStatus = profile.CompanyUserAccountStatus;
                var blockStatus = profile.CompanyUserBlockStatus;

                var partner = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(businessProfileCode);

                List<SubscriptionAccountStatus> subscriptionAccountStatuses = new List<SubscriptionAccountStatus>();
                var subscriptions = await _partnerRepository.GetSubscriptionsByPartnerCodeAsync(partner.Id);

                string solutionPrefix = "";
                foreach (var s in subscriptions)
                {
                    var subscriptionAccountStatus = new SubscriptionAccountStatus()
                    {
                        PartnerSubscriptionCode = s.Id,
                        PartnerAccountStatusType = s.PartnerAccountStatusType?.Id
                    };
                    subscriptionAccountStatuses.Add(subscriptionAccountStatus);

                    if (s.Solution == Solution.Connect)
                    {
                        solutionPrefix = "connect";
                    }
                    else if (s.Solution == Solution.Business)
                    {
                        solutionPrefix = "business";
                    }

                    string content = JsonConvert.SerializeObject(subscriptionAccountStatuses);

                    //TODELETE
                    //_Claims.Add(new Claim($"company_name.{companyId}", companyName, ClaimValueTypes.String));
                    var compKeyClaim = $"{solutionPrefix}.comp.{companyId}";
                    _Claims.Add(new Claim(compKeyClaim, companyName, ClaimValueTypes.String));

                    if (accountStatus != null)
                    {
                        //TODELETE
                        //_Claims.Add(new Claim($"account_status.{companyId}.{accountStatus.Id}", accountStatus.Name, ClaimValueTypes.String));
                        _Claims.Add(new Claim($"{solutionPrefix}.act_stat.{companyId}", accountStatus.Id.ToString(), ClaimValueTypes.String));
                    }
                    if (blockStatus != null)
                    {
                        //_Claims.Add(new Claim($"block_status.{companyId}.{blockStatus.Id}", blockStatus.Name, ClaimValueTypes.String));
                        _Claims.Add(new Claim($"{solutionPrefix}.blck_stat.{companyId}", blockStatus.Id.ToString(), ClaimValueTypes.String));
                    }
                    //TO ADD ( To retrieve from Partner Account Status )


                    //_Claims.Add(new Claim($"comp_stat.{companyId}", partner.PartnerAccountStatusType.Id.ToString(), ClaimValueTypes.String));
                    _Claims.Add(new Claim($"{solutionPrefix}.comp_stat.{companyId}", content, JsonClaimValueTypes.JsonArray));
                    _Claims.Add(new Claim($"{solutionPrefix}.comp_partner.{companyId}", partner.Id.ToString(), ClaimValueTypes.String));

                    var customerUserBusinessProfileRoleSpec = Specification<CustomerUserBusinessProfileRole>.All;
                    var roleByCustomerUserBusinessProfileCode = new RoleByCustomerUserBusinessProfileCode(customerUserBusinessProfileCode);
                    customerUserBusinessProfileRoleSpec = customerUserBusinessProfileRoleSpec.And(roleByCustomerUserBusinessProfileCode);
                    var customerUserBusinessProfileCodeList = await Repository.GetCustomerUserBusinessProfilesRolesAsync(customerUserBusinessProfileRoleSpec);

                    StringBuilder roles = new StringBuilder();
                    StringBuilder roleCodes = new StringBuilder();

                    //TODO: Fix the n+1 problem
                    //TODO: Do not trigger .Result directly

                    foreach (var profileCode in customerUserBusinessProfileCodeList)
                    {
                        var roleCode = profileCode.RoleCode;
                        var externalRole = await _externalUserRoleRepository.GetExternalRoleByRoleCodeAsync(roleCode);

                        if (externalRole != null)
                        {
                            if (profileCode != customerUserBusinessProfileCodeList.Last())
                            {
                                roles.Append($"{externalRole.ExternalUserRoleName},");
                                roleCodes.Append($"{roleCode},");
                            }
                            else
                            {
                                roles.Append(externalRole.ExternalUserRoleName);
                                roleCodes.Append(roleCode);
                            }
                        }
                    }

                    _Claims.Add(new Claim($"{solutionPrefix}.role.{companyId}", roles.ToString(), ClaimValueTypes.String));
                    _Claims.Add(new Claim($"{compKeyClaim}.role_code.", roleCodes.ToString(), ClaimValueTypes.String));
                }
            }
            #endregion

            return _Claims;
        }

        public override Task<string> GetEmailAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Email.Value);
        }

        /*
        public override Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }
        */
        public override async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user is TrangloStaff)
            {
                return new List<string> { "InternalUser" };
            }

            return await base.GetRolesAsync(user);
        }

        public async Task<string> GetClaimsCombinationUniqueFile(ApplicationUser user)
        {
            var claims = await this.GetClaimsAsync(user);

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var type = claims.FirstOrDefault(x => x.Type == "type");
            var jti = claims.FirstOrDefault(x => x.Type == "login_time");

            string username = "";
            var splitSubs = sub.Value.Split("@");
            if (splitSubs.Length > 0)
                username = splitSubs[0];

            string fileName = String.Concat(type.Value, "_", username, "_", jti.Value);

            return fileName;
        }

        public async Task<IdentityResult> ConfirmInviteeEmailAsync(ApplicationUser user, string token, CancellationToken cancellationToken)
        {
            if (user is TrangloStaff)
            {
                return await Task.FromResult<IdentityResult>(null);
            }

            if (!await base.VerifyUserTokenAsync(user,
                Options.Tokens.EmailConfirmationTokenProvider, ConfirmEmailTokenPurpose, token))
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidToken());
            }

            CustomerUser customer = user as CustomerUser;
            customer.ConfirmInviteeEmail();

            //Set CustomerUserBusinessProfile AccountStatus to Active
            var byUserID = new CustomerUserBusinessProfileByUserID(user.Id);
            var customerUserBusinessProfileList = await Repository.GetCustomerUserBusinessProfilesAsync(byUserID);

            foreach (var profile in customerUserBusinessProfileList)
            {
                profile.SetCompanyUserAccountStatus(CompanyUserAccountStatus.Active);
            }
            await Repository.UpdateCustomerUserBusinessProfileRangeAsync(customerUserBusinessProfileList.ToList(), cancellationToken);

            var result = await _applicationUserRepository.UpdateApplicationUser(customer, cancellationToken);
            if (result.IsFailure)
            {
                IdentityResult.Failed(new IdentityError
                {
                    Description = $"Unable to set Account Status to Active for {customer.Email}"
                });
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token, int? solutionCode, bool isMultipleSolution)
        {
            if (user is TrangloStaff)
            {
                return await Task.FromResult<IdentityResult>(null);
            }

            if (!await base.VerifyUserTokenAsync(user,
                Options.Tokens.EmailConfirmationTokenProvider, ConfirmEmailTokenPurpose, token))
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidToken());
            }

            CustomerUser customer = user as CustomerUser;
            customer.ConfirmEmail(solutionCode, isMultipleSolution);

            return await UpdateUserAsync(user);
        }

        public override async Task<string> GenerateChangeEmailTokenAsync(ApplicationUser user, string newEmail)
        {
            return user is TrangloStaff ? await Task.FromResult<string>(null) : await base.GenerateChangeEmailTokenAsync(user, newEmail);
        }

        //public override async FindByIdAsync(string userId)
        public async Task<Result<int>> InviteAsync(
            ApplicationUser inviter,
            FullName inviteeFullName,
            Email inviteeEmail,
            List<string> userRoleCodes,
            string passwordHash,
            int businessProfileCode,
            string timezone,
            int solutionCode)
        {
            //persist invitation
            //Event : InvitationSubmittedEvent
            //  -> create customer user without RegisteredEvent.
            //  -> send invitation email

            //Find inviter by loginId provided
            CustomerUser inviterChecker = await base.FindByIdAsync(inviter.LoginId) as CustomerUser;
            // Find user by email provided
            //CustomerUser invitee = await base.FindByIdAsync(inviteeEmail.Value) as CustomerUser; //To recheck, causing entity tracking issue on CountryMeta

            CustomerUser invitee = await _applicationUserRepository.GetCustomerUserAsync(inviteeEmail.Value);

            ApplicationUser applicationUser = await _applicationUserRepository.GetApplicationUserByLoginId(inviteeEmail.Value);
            bool isNewCustomerUser = false;
            if (applicationUser == null)
            {
                isNewCustomerUser = true;
            }
            else if (applicationUser.AccountStatus == AccountStatus.PendingActivation)
            {
                isNewCustomerUser = true;
            }

            BusinessProfile businessProfile = await businessProfileRepository.GetBusinessProfileByCodeAsync(businessProfileCode);
            CustomerUserRegistration customerUserRegistration = await _applicationUserRepository.GetCustomerUserRegistrationsByCompanyNameAndLoginIdAndSolutionAsync(businessProfile.CompanyName, inviteeEmail.Value, solutionCode);
            CustomerUser _Customer;

            if (invitee == null)
            {
                var _CustomerUserRegistration = new CustomerUserRegistration(inviteeEmail, CompanyName.Create(businessProfile.CompanyName).Value, businessProfileCode, solutionCode);
                await _applicationUserRepository.AddCustomerUserRegistration(_CustomerUserRegistration);
                if (inviterChecker != null)
                {
                    _Customer = CustomerUser.Create(inviteeFullName, inviteeEmail, passwordHash, inviterChecker.CountryMeta, timezone);

                }
                else
                {
                    _Customer = CustomerUser.Create(inviteeFullName, inviteeEmail, passwordHash, null, timezone);
                }
                IdentityResult RegisterUserResult = await base.CreateAsync(_Customer, passwordHash);

                if (!RegisterUserResult.Succeeded)
                {
                    _logger.LogError("CreateAsync", $"Create User [{inviteeEmail}] failed. {RegisterUserResult.Errors}");
                    return Result.Failure<int>($"Create User [{inviteeEmail}] failed. {RegisterUserResult.Errors}");
                }

                invitee = _Customer;
            }
            else
            {
                if (customerUserRegistration == null)
                {
                    var _CustomerUserRegistration = new CustomerUserRegistration(inviteeEmail, CompanyName.Create(businessProfile.CompanyName).Value, businessProfileCode, solutionCode);
                    await _applicationUserRepository.AddCustomerUserRegistration(_CustomerUserRegistration);

                    //Invitation userInvitations = new Invitation(
                    //                                    inviter,
                    //                                    businessProfileCode,
                    //                                    invitee.FullName,
                    //                                    invitee.Email,
                    //                                    isNewCustomerUser,
                    //                                    businessProfile.CompanyName
                    //                                );
                    //
                    //// Save invitation in DB
                    //Invitation invitationResponses = _invitationRepository.AddInvitations(userInvitations);

                    //return Result.Success(invitationResponses.Id);

                    // return Result.Success<int>(1);

                }
                else
                {
                    _logger.LogError($" User [{inviteeEmail.Value}] is already existed with Company [{customerUserRegistration.CompanyName}].");
                    return Result.Failure<int>($" User [{inviteeEmail.Value}] is already existed with Company [{customerUserRegistration.CompanyName}].");
                }
            }

            bool isNewApplication = false;
            if (customerUserRegistration == null)
            {
                isNewApplication = true;
            }

            Result<IReadOnlyList<CustomerUserBusinessProfile>> customerUserBusinessProfiles = await _businessProfileService
                                                                                                    .GetCustomerUserBusinessProfilesAsync(
                                                                                                        invitee,
                                                                                                        businessProfileCode
                                                                                                    );

            // Return error if user already exist in that company
            if (customerUserBusinessProfiles.IsSuccess && customerUserBusinessProfiles.Value.Count > 0 && customerUserRegistration != null)
            {
                _logger.LogError("GetCustomerUserBusinessProfiles", $"User [{invitee.Email.Value}] already exist with business profile code: {businessProfileCode}");
                return Result.Failure<int>($"User [{invitee.Email.Value}] already exist with business profile code: {businessProfileCode}");
            }

            #region Create company user business profile and role
            //Mock User Role
            //UserRole mockUserRoleResult = Enumeration.FindById<UserRole>(3);
            //var roleCode = 3;
            Result<BusinessProfile> createCustomerUserBusinessProfileResult = await _businessProfileService.CreateCustomerUserBusinessProfileAndRoleAsync(
                                                                                        invitee,
                                                                                        businessProfileCode,
                                                                                        userRoleCodes,
                                                                                        isNewApplication
                                                                                    );

            // Return error if user already exist in that company
            if (createCustomerUserBusinessProfileResult.IsFailure)
            {
                _logger.LogError("CreateCustomerUserBusinessProfile", $"CreateCustomerUserBusinessProfile failed for user [{invitee.Email.Value}]");
                return Result.Failure<int>($"CreateCustomerUserBusinessProfile failed for user [{invitee.Email.Value}]");
            }

            // Mock Customer Business Profile
            //int mockBusinessProfileCode = 25;
            //var businessProfilesSpec = Specification<BusinessProfile>.All;
            //var companyNameByBusinessProfileCode = new CompanyNameByBusinessProfileCode(mockBusinessProfileCode);
            //businessProfilesSpec = businessProfilesSpec.And(companyNameByBusinessProfileCode);
            //var businessProfileList = await Repository.GetBusinessProfilesAsync(businessProfilesSpec);
            //var mockBusinessProfile = businessProfileList.SingleOrDefault();

            _logger.LogInformation($"Business profile has been created for customer [{invitee.Email.Value}]");
            #endregion

            var customerBusinessProfile = await businessProfileRepository.GetBusinessProfileByCodeAsync(businessProfileCode);

            #region Create user invitation
            Invitation userInvitation = new Invitation(
                                                        inviter,
                                                        businessProfileCode,
                                                        invitee.FullName,
                                                        invitee.Email,
                                                        isNewCustomerUser,
                                                        customerBusinessProfile.CompanyName
                                                    );

            // Save invitation in DB
            Invitation invitationResponse = _invitationRepository.AddInvitations(userInvitation);

            return Result.Success(invitationResponse.Id);
            #endregion
        }
        public async Task<Result<int>> ResendInvitationAsync(
          ApplicationUser inviter,
          int? businessProfileCode,
          Email inviteeEmail
          )
        {

            // Find user by email provided
            ApplicationUser invitee = await base.FindByIdAsync(inviteeEmail.Value) as ApplicationUser;

            bool isNewCustomerUser = true;

            var customerUserBusinessProfile = await businessProfileRepository.GetBusinessProfileByCodeAsync(businessProfileCode);

            var customerUserProfileResult = await _applicationUserRepository.GetCustomerUserRegistrationsByLoginIdAsync(inviteeEmail.Value);

            var companyName = string.Empty;

            if (businessProfileCode > 0)
            {
                companyName = customerUserBusinessProfile.CompanyName;
            }
            else
            {
                companyName = customerUserProfileResult.CompanyName;
                businessProfileCode = null;
            }

            Invitation userInvitation = new Invitation(
                                                        inviter,
                                                        businessProfileCode,
                                                        invitee.FullName,
                                                        invitee.Email,
                                                        isNewCustomerUser,
                                                        companyName
                                                    );
            Invitation invitationResponse = _invitationRepository.AddInvitations(userInvitation);

            return Result.Success(userInvitation.Id);



        }

        //public async Task<List<EmailRecipient>> GetRequisitionRecipientEmailAsync(string permissionInfoCode,int approvalLevel, string trangloEntity, bool isProduction)
        //{
        //    var _connectionString = _config.GetConnectionString("DefaultConnection");
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        var reader = (await connection.QueryAsync<EmailRecipient>(
        //                "GetRequisitionApprovalEmail",
        //                new
        //                {
        //                    @PermissionInfoCode = permissionInfoCode,
        //                    @AuthorityLevelCode = approvalLevel,
        //                    @TrangloEntity = trangloEntity,
        //                    @IsProduction = isProduction
        //                },
        //              commandType: CommandType.StoredProcedure)).ToList();
        //
        //        return reader;
        //    }
        //}

        public override async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser user, int number)
        {
            var newCodes = new List<string>();
            var random = new Random();

            for (int i = 0; i < number; i++)
            {
                var code = Guid.NewGuid().ToString("N").Substring(0, 8);
                var randomizedCode = new string(code.Select(c => random.Next(2) == 0 ? char.ToUpper(c) : char.ToLower(c)).ToArray());
                newCodes.Add(randomizedCode);
            }

            await _applicationUserRepository.ReplaceCodesAsync(user, newCodes);
            return newCodes;
        }

        public override async Task<int> CountRecoveryCodesAsync(ApplicationUser user)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }

            return await _applicationUserRepository.CountValidRecoveryCode(user);
        }

        public async Task<List<PartnerSubscription>> GetPartnerSubscriptionsForUserAsync(CustomerUser customerUser)
        {
            return await _businessProfileService.GetPartnerSubscriptionByUserIdAsync(customerUser.Id);
        }

        public static class ClaimCode
        {
            public const string IS_NEW_USER = "is_new_user";
        }
    }
}
