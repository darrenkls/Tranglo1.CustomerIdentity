using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.ActiveDirectory;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.Repositories
{
	class LdapAccountRepository : ILdapAccountRepository
	{
		public LdapAccountRepository(LdapAccountDbContext ldapAccountDbContext)
		{
			LdapAccountDbContext = ldapAccountDbContext ?? throw new ArgumentNullException(nameof(ldapAccountDbContext));
		}

		public LdapAccountDbContext LdapAccountDbContext { get; }

		public async Task<IReadOnlyCollection<LdapAccount>> GetLdapAccountsAsync()
		{
			return await this.LdapAccountDbContext.LdapAccounts.ToListAsync();
		}

		public async Task SaveAsync(LdapAccount account)
		{
			var e = this.LdapAccountDbContext.Entry(account);

			if (e.State == EntityState.Detached)
			{
				e.State = EntityState.Added;
			}
			else
			{
				e.State = EntityState.Modified;
			}

			await this.LdapAccountDbContext.SaveChangesAsync();
		}
	}
}
