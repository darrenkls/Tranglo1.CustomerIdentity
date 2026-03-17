using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes;
using Tranglo1.CustomerIdentity.Infrastructure.Event;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.Infrastructure.Persistence
{
    public class SignUpCodeDBContext : BaseDbContext
    {
        public const string DEFAULT_SCHEMA = "dbo";
        public const string META_SCHEMA = "meta";
        public const string HISTORY_SCHEMA = "history";

        public DbSet<SignUpCode> SignUpCodes { get; set; }
        public DbSet<SignUpAccountStatus> SignUpAccountStatuses { get; set; }
        public DbSet<LeadsOrigin> LeadsOrigins { get; set; }

        public SignUpCodeDBContext(
                 DbContextOptions<SignUpCodeDBContext> options,
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
