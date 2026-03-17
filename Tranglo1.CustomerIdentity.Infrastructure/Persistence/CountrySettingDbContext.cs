using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Events;
using Tranglo1.CustomerIdentity.Infrastructure.Event;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.Infrastructure.Persistence
{
    public class CountrySettingDbContext : BaseDbContext
    {
        public const string DEFAULT_SCHEMA = "dbo";
        public const string META_SCHEMA = "meta";
        public const string HISTORY_SCHEMA = "history";

        public DbSet<CountrySetting> CountrySettings { get; set; } 
        public DbSet<CountryMeta> CountryMetas { get; set; } 
        public DbSet<CountrySettingsChangedEvent> CountrySettingsChangedEvents { get; set; }

        public CountrySettingDbContext(
                 DbContextOptions<CountrySettingDbContext> options,
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
