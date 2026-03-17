using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.ActiveDirectory;
using Tranglo1.CustomerIdentity.Infrastructure.Event;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.Infrastructure.Persistence
{
	public class LdapAccountDbContext : BaseDbContext
	{
		public DbSet<LdapAccount> LdapAccounts { get; set; }

		public LdapAccountDbContext(DbContextOptions<LdapAccountDbContext> dbContextOptions,
			IEventDispatcher dispatcher,
			IUnitOfWork unitOfWorkContext,
			IIdentityContext identityContext)
			: base(dbContextOptions, dispatcher, unitOfWorkContext, identityContext)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
		}
	}
}
