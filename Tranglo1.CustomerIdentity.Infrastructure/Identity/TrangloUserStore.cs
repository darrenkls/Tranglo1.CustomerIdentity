using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.Identity
{
    public class TrangloUserStore : ITrangloUserStore
    {
        private readonly ApplicationUserDbContext applicationUserDbContext;

        public TrangloUserStore(ApplicationUserDbContext applicationUserDbContext)
        {
            this.applicationUserDbContext = applicationUserDbContext;
        }

        private ApplicationUserDbContext Context => this.applicationUserDbContext;

        public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                var userClaim = new ApplicationUserClaim(user.Id);
                userClaim.InitializeFromClaim(claim);
                this.Context.ApplicationUserClaims.Add(userClaim);
            }

            await Context.SaveChangesAsync(cancellationToken);
        }

        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        public async Task<int> CountCodesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var entry = await GetMFAAsync(user, cancellationToken);
            if (entry.RecoveryCode?.Length > 0)
            {
                return entry.RecoveryCode.Split(';').Length;
            }
            return 0;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                var entry = await this.Context.ApplicationUsers.AddAsync(user, cancellationToken);
                /*CountryMeta-YY
                if (user is CustomerUser customerUser)
                {
                    this.Context.Entry(customerUser.Country).State = EntityState.Unchanged;
                }
                */
                await this.Context.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.ToString() });
            }
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                Context.ApplicationUsers.Remove(user);
                await Context.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.ToString() });
            }
        }

        public void Dispose()
        {
            this.Context.Dispose();
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var searchResult = from user in BuildUserQuery()
                               where user.Email.Value == normalizedEmail
                               select user;

            return await searchResult.FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var _search = from u in BuildUserQuery()
                          where u.FullName.Value == normalizedUserName
                          select u;

            return await _search.FirstOrDefaultAsync();
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<string> GetAuthenticatorKeyAsync(ApplicationUser user, CancellationToken cancellationToken)
            => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var claims = from c in Context.ApplicationUserClaims
                         where c.UserId == user.Id
                         select c.ToClaim();

            return await claims.ToListAsync();
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email.Value);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            bool emailConfirmed = false;

            if (user is CustomerUser customerUser)
            {
                emailConfirmed = customerUser.EmailConfirmed;
            }
            else if (user is TrangloStaff)
            {
                emailConfirmed = true;
            }

            return Task.FromResult(emailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email.Value.ToLowerInvariant());
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.FullName.Value.ToLowerInvariant());
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string passwordHash = "";
            if (user is CustomerUser customerUser)
                passwordHash = customerUser.PasswordHash;

            return Task.FromResult(passwordHash);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string phoneNumber = "";
            if (user is CustomerUser customerUser)
                phoneNumber = customerUser.ContactNumber?.Value;

            return Task.FromResult(phoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
            //throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public async Task<string> GetTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var result = GetMFAAsync(user, cancellationToken);
            return await Task.FromResult(result?.Result?.Token);
        }
        public async Task<MFA> GetMFAAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var entry = await applicationUserDbContext.MFA
                .Where(t => t.UserId == user.Id).FirstOrDefaultAsync();

            return await Task.FromResult(entry);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LoginId);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.FullName.Value);
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var query = from userclaims in Context.ApplicationUserClaims
                        join user in BuildUserQuery() on userclaims.UserId equals user.Id
                        where userclaims.ClaimValue == claim.Value
                        && userclaims.ClaimType == claim.Type
                        select user;

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string passwordHash = null;
            if (user is CustomerUser customerUser)
                passwordHash = customerUser.PasswordHash;
            return await Task.FromResult(passwordHash != null);
        }

        public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            int userAccessFailedCount = user.AccessFailedCount;
            user.SetAccessFailedCount(userAccessFailedCount + 1);
            return await Task.FromResult(user.AccessFailedCount);
        }

        public async Task<bool> RedeemCodeAsync(ApplicationUser user, string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }

        public async Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var _MatchedClaims = await (from c in Context.ApplicationUserClaims
                                        where c.Id == user.Id
                                        select c).ToListAsync(cancellationToken);

            foreach (var claim in _MatchedClaims)
            {
                if (claims.Any(c => c.Type == claim.ClaimType && c.Value == claim.ClaimValue))
                {
                    Context.ApplicationUserClaims.Remove(claim);
                }
            }

            await Context.SaveChangesAsync(cancellationToken);
        }

        public Task RemoveTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            //
            //cancellationToken.ThrowIfCancellationRequested();
            //ThrowIfDisposed();

            //if (user == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            //var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            //if (entry != null)
            //{
            //    await RemoveUserTokenAsync(entry);
            //}
            //

            throw new NotImplementedException();
        }

        public async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            var _MatchedClaims = await (from c in Context.ApplicationUserClaims
                                        where
                                            c.ClaimType == claim.Type &&
                                            c.ClaimValue == claim.Value &&
                                            c.UserId == user.Id
                                        select c).ToListAsync(cancellationToken);

            foreach (var c in _MatchedClaims)
            {
                c.ClaimType = newClaim.Type;
                c.ClaimValue = newClaim.Value;
            }

            await Context.SaveChangesAsync(cancellationToken);
        }

        public Task ReplaceCodesAsync(ApplicationUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            var mergedCodes = string.Join(";", recoveryCodes);
            return SetRecoveryCodeAsync(user, mergedCodes, cancellationToken);
        }

        public async Task SetRecoveryCodeAsync(ApplicationUser user, string recoveryCodes, CancellationToken cancellationToken)
        {
            var entry = await GetMFAAsync(user, cancellationToken);
            entry.RecoveryCode = recoveryCodes;
            applicationUserDbContext.Entry(entry).State = EntityState.Modified;

            applicationUserDbContext.MFA.Update(entry);
            await applicationUserDbContext.SaveChangesAsync();

        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.SetAccessFailedCount(0);
            return Task.CompletedTask;
        }

        public Task SetAuthenticatorKeyAsync(ApplicationUser user, string key, CancellationToken cancellationToken)
            => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.SetEmail(Email.Create(email).Value);
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            //cancellationToken.ThrowIfCancellationRequested();
            //ThrowIfDisposed();
            //if (user == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            //user.EmailConfirmed = confirmed;
            //return Task.CompletedTask;

            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.SetLockoutEnabled(enabled);
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

			if (lockoutEnd.HasValue)
            {
                var lockoutEndDate = DateTimeOffset.MaxValue;
                user.SetLockoutEnd(lockoutEndDate);
			}
            else
            {
                user.SetLockoutEnd(null);
			}

            return Task.CompletedTask;
        }



        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user is CustomerUser customerUser)
                customerUser.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            //cancellationToken.ThrowIfCancellationRequested();
            //ThrowIfDisposed();
            //if (user == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            //user.PhoneNumber = phoneNumber;
            //return Task.CompletedTask;

            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            //cancellationToken.ThrowIfCancellationRequested();
            //ThrowIfDisposed();
            //if (user == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            //user.PhoneNumberConfirmed = confirmed;
            //return Task.CompletedTask;

            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SetSecurityStamp(stamp);
            return Task.CompletedTask;
        }

        public async Task SetTokenAsync(ApplicationUser user, string loginProvider, string authenticatorKeyTokenName, string token, CancellationToken cancellationToken)
        {
            var result = await GetMFAAsync(user, cancellationToken);

            if (result != null)
            {
                result.AuthenticationType = AuthenticationType.Authenticator_Application;
                result.Token = token;
                result.RecoveryCode = null;
                applicationUserDbContext.MFA.Update(result);
            }
            else
            {
                var mfa = new MFA();

                mfa.AuthenticationType = AuthenticationType.Authenticator_Application;
                mfa.UserId = user.Id;
                mfa.Token = token;
                mfa.RecoveryCode = null;
                applicationUserDbContext.MFA.Add(mfa);
            }

            await applicationUserDbContext.SaveChangesAsync();
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.SetTwoFactorEnabled(enabled); //user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.SetName(FullName.Create(userName).Value);
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            /*
            if (user is CustomerUser customerUser)
            {
                this.Context.Entry(customerUser.AccountStatus).State = EntityState.Unchanged;
            }
            var updateResult = Context.ApplicationUsers.Update(user);
            */
            Context.ApplicationUsers.Attach(user);
            await Context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            Result<Email> result = Email.Create(userId);

            if (result.IsSuccess)
            {
                IQueryable<ApplicationUser> searchResult = from u in BuildCustomerUserQuery()
                                                           where u.LoginId == userId
                                                           select u;

                return await searchResult.FirstOrDefaultAsync(cancellationToken);
            }
            else
            {
                IQueryable<ApplicationUser> searchResult = from u in BuildTrangloStaffQuery()
                                                           where u.LoginId == userId
                                                           select u;

                return await searchResult.FirstOrDefaultAsync(cancellationToken);
            }
        }

        private IQueryable<TrangloStaff> BuildTrangloStaffQuery()
        {
            return this.Context.ApplicationUsers.OfType<TrangloStaff>()
                .Include(e => e.AccountStatus);
        }

        private IQueryable<CustomerUser> BuildCustomerUserQuery()
        {
            return this.Context.ApplicationUsers.OfType<CustomerUser>()
                .Include(e => e.AccountStatus);
                //.Include(e => e.Solution)
            //.Include(e => e.Country)
            //.Include(e => e.UserType);
        }

        private IQueryable<ApplicationUser> BuildUserQuery()
        {
            return this.Context.ApplicationUsers
                .Include(e => e.AccountStatus);
                //.Include(e => ((CustomerUser)e).Solution)
            //.Include(e => ((CustomerUser)e).Country)
            //.Include(e => ((CustomerUser)e).UserType);

        }
    }
}