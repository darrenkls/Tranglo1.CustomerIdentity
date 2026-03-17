
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.ActiveDirectory
{
	public class LdapAccountMergeResults
	{
		private readonly List<LdapAccount> _newAccounts;
		private readonly List<LdapAccount> _deletedAccounts;
		private readonly List<LdapAccount> _updatedAccounts;

		public IReadOnlyCollection<LdapAccount> NewAccounts => _newAccounts.AsReadOnly();
		public IReadOnlyCollection<LdapAccount> DeletedAccounts => _deletedAccounts.AsReadOnly();
		public IReadOnlyCollection<LdapAccount> UpdatedAccounts => _updatedAccounts.AsReadOnly();

		private readonly IReadOnlyCollection<LdapAccount> _EmptyList = new List<LdapAccount>();

		public LdapAccountMergeResults()
		{
			this._newAccounts = new List<LdapAccount>();
			this._deletedAccounts = new List<LdapAccount>();
			this._updatedAccounts = new List<LdapAccount>();
		}
		
		public void AddNewAccount(LdapAccount ldapAccount)
		{
			if (ldapAccount is null)
			{
				throw new ArgumentNullException(nameof(ldapAccount));
			}

			this._newAccounts.Add(ldapAccount);
		}

		public void AddDeletedAccount(LdapAccount ldapAccount)
		{
			if (ldapAccount is null)
			{
				throw new ArgumentNullException(nameof(ldapAccount));
			}

			this._deletedAccounts.Add(ldapAccount);
		}

		public void AddUpdatedAccount(LdapAccount ldapAccount)
		{
			if (ldapAccount is null)
			{
				throw new ArgumentNullException(nameof(ldapAccount));
			}

			this._updatedAccounts.Add(ldapAccount);
		}

	}
}
