using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Domain.Entities.ActiveDirectory;

namespace Tranglo1.CustomerIdentity.Domain.DomainServices
{
	public class LdapAccountManager
	{
		public LdapAccountManager(ILdapAccountRepository accountRepository)
		{
			AccountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
		}

		public ILdapAccountRepository AccountRepository { get; }

		public async Task<LdapAccountMergeResults> MergeLdapAccountsAsync(IEnumerable<LdapAccount> latest)
		{
			LdapAccountMergeResults mergeResults = new LdapAccountMergeResults();

			var _ExistingLdapAccountsTask = this.AccountRepository.GetLdapAccountsAsync();

			Dictionary<string, LdapAccount> _fromLdap = latest.ToDictionary(a => a.SamAccountName.ToLower(), a => a);
			Dictionary<string, LdapAccount> _fromSystem = (await _ExistingLdapAccountsTask).ToDictionary(a => a.SamAccountName.ToLower(), a => a);

			if (_fromLdap.Any() && _fromSystem.Any() == false)
			{
				//First time load from Ldap server
				foreach (var item in _fromLdap)
				{
					//await this.AccountRepository.SaveAsync(item.Value);

					if (item.Value.IsEnabled)
					{
						mergeResults.AddNewAccount(item.Value);
					}
				}
			}
			else
			{
				foreach (var item in _fromLdap)
				{
					if (_fromSystem.TryGetValue(item.Key, out var _inSystem) == false)
					{
						//detect new in LDAP, but not in current system
						//await this.AccountRepository.SaveAsync(item.Value);
						mergeResults.AddNewAccount(item.Value);

					}
					else
					{
						//the account exist in both side, so we just check email, fullname and status 
						//are the same or not

						if (_inSystem.IsEnabled == true && _inSystem.IsEnabled != item.Value.IsEnabled)
						{
							//deleted in Ldap, and exists in current system
							_inSystem.IsEnabled = item.Value.IsEnabled;
							mergeResults.AddDeletedAccount(_inSystem);
						}
						else if (_inSystem.IsEnabled == true && item.Value.IsEnabled == true)
						{
							bool _IsUpdated = false;

							if (string.Equals(_inSystem.EmailAddress, item.Value.EmailAddress,
								StringComparison.OrdinalIgnoreCase) == false)
							{
								_inSystem.EmailAddress = item.Value.EmailAddress;
								_IsUpdated = true;
							}

							if (string.Equals(_inSystem.Name, item.Value.Name, StringComparison.OrdinalIgnoreCase) == false)
							{
								_inSystem.Name = item.Value.Name;
								_IsUpdated = true;
							}

							if (_IsUpdated)
							{
								mergeResults.AddUpdatedAccount(_inSystem);
							}
						}
						else if (_inSystem.IsEnabled == false && item.Value.IsEnabled == true) 
						{
							//specific for re-joiner where disabled in current system and enable in ldap
							_inSystem.IsEnabled = item.Value.IsEnabled;
							mergeResults.AddUpdatedAccount(_inSystem);
						}
					}
				}

				//found in current system, but not found in LDAP
				foreach (var item in _fromSystem)
				{
					if (_fromLdap.ContainsKey(item.Key) == false)
					{
						if (item.Value.IsEnabled)
						{
							item.Value.IsEnabled = false;
							mergeResults.AddDeletedAccount(item.Value);
						}
					}
				}
			}

			return mergeResults;
		}
	}
}
