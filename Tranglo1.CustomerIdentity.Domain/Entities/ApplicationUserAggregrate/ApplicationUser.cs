using System;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    /// <summary>
	/// General User class for both internal and external user. Please note that
	/// <c>Id</c> : This is the unique identifier for all users, also known as Login Id. For internal user, this is the AD account (ex: xxx@tranglo.net)
	/// <c>UserName</c> : This is the user's friendly name, not used as login Id.
	/// </summary>
    public abstract class ApplicationUser : AggregateRoot
	{
        protected ApplicationUser()
        {

        }
		public ApplicationUser(FullName fullName, Email email )
		{
			SetName(fullName);
			SetEmail(email);
        }
		public ApplicationUser(long accountStatus)
        {

        }
		public FullName FullName { get; private set; }
		public AccountStatus AccountStatus { get; private set; }
		public long AccountStatusCode { get; protected set; }
		public string Timezone { get; set; }
        public virtual DateTimeOffset? LockoutEnd { get; private set; }
		public virtual bool TwoFactorEnabled { get; private set; }
		public virtual ContactNumber ContactNumber { get; private set; }
		public virtual string ConcurrencyStamp { get; private set; }
		public virtual string SecurityStamp { get; private set; }
		public virtual Email Email { get; protected set; }

		/// <summary>
		/// This is the login Id
		/// </summary>
		public virtual string LoginId { get; protected set; }
		public virtual bool LockoutEnabled { get; private set; }
		public virtual int AccessFailedCount { get; private set; }
		public virtual bool? IsResetMFA { get; private set; }


		public void SetName(FullName fullname)
		{
			this.FullName = fullname;
		}

		public virtual void SetEmail(Email email)
		{
			this.Email = email;
		}

		public void SetConcurrencyStamp(string concurrencyStamp)
        {
			this.ConcurrencyStamp = concurrencyStamp;
        }
		public void SetTwoFactorEnabled(bool twoFactorEnabled)
		{
			this.TwoFactorEnabled = twoFactorEnabled;
		}
		public void SetLockoutEnabled(bool lockoutEnabled)
		{
			this.LockoutEnabled = lockoutEnabled;
		}
		public void SetLockoutEnd(DateTimeOffset? lockoutEnd)
        {
			this.LockoutEnd = lockoutEnd;
		}
		public void SetAccessFailedCount(int accessFailedCount )
        {
			this.AccessFailedCount = accessFailedCount;
		}
		public void SetSecurityStamp( string securityStamp )
        {
			this.SecurityStamp = securityStamp;
		}
		public void SetAccountStatus(AccountStatus accountStatus)
		{
			this.AccountStatus = accountStatus;
			this.AccountStatusCode = accountStatus.Id;
		}

		// Used in TB portal invite user
		public void SetTimezone(string timezone)
		{
			this.Timezone = timezone;
		}

		public void SetIsResetMFA(bool isResetMFA)
		{
			IsResetMFA = isResetMFA;
		}
	}
}