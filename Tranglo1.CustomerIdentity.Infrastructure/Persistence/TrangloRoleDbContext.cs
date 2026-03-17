using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration;
using Tranglo1.CustomerIdentity.Infrastructure.Event;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using UserType = Tranglo1.CustomerIdentity.Domain.Entities.UserType;

namespace Tranglo1.CustomerIdentity.Infrastructure.Persistence
{
	public class TrangloRoleDbContext : BaseDbContext//, ITrangloRoleDbContext
	{
		public const string DEFAULT_SCHEMA = "dbo";
		public const string META_SCHEMA = "meta";
		public const string HISTORY_SCHEMA = "history";

		//entities
		public DbSet<TrangloRole> TrangloRoles { get; set; }

		//meta
		public DbSet<TrangloEntity> TrangloEntities { get; set; }
		public DbSet<TrangloStaffEntityAssignment> TrangloStaffEntityAssignments { get; set; }
		public TrangloRoleDbContext(
			DbContextOptions<TrangloRoleDbContext> options,
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

