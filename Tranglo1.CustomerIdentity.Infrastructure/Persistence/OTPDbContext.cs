using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.OTP;
using Tranglo1.CustomerIdentity.Infrastructure.Event;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.Infrastructure.Persistence
{
	public class OTPDbContext : BaseDbContext
	{
		public OTPDbContext(
			DbContextOptions<OTPDbContext> options, IEventDispatcher dispatcher,
			IUnitOfWork unitOfWorkContext, IIdentityContext identityContext)
			: base(options, dispatcher, unitOfWorkContext, identityContext)
		{

		}

		public const string DEFAULT_SCHEMA = "dbo";
		public const string META_SCHEMA = "meta";
		public const string HISTORY_SCHEMA = "history";

		protected override void OnModelCreating(ModelBuilder builder)
		{
			//System.Diagnostics.Debugger.Launch();
			base.OnModelCreating(builder);

			builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

			
		}
		public DbSet<RequisitionOTP> RequisitionOTPs { get; set; }
		

	}
}
