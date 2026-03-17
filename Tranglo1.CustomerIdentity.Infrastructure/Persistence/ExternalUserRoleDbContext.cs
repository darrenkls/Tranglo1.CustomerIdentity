using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.ExternalUserRoleAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Event;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.Infrastructure.Persistence
{
    public class ExternalUserRoleDbContext : BaseDbContext
    {
		public const string DEFAULT_SCHEMA = "dbo";
		public const string META_SCHEMA = "meta";
		public const string HISTORY_SCHEMA = "history";

		// Entities
		public DbSet<ExternalUserRole> ExternalUserRoles { get; set; }

		public ExternalUserRoleDbContext(
			DbContextOptions<ExternalUserRoleDbContext> options,
			IUnitOfWork unitOfWorkContext,
			IEventDispatcher dispatcher, IIdentityContext identityContext)
			: base(options, dispatcher, unitOfWorkContext, identityContext)
		{

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
		}
	}
}
