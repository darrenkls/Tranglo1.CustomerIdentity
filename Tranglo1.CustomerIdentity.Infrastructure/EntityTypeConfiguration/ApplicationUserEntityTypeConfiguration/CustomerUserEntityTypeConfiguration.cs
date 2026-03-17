using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class CustomerUserEntityTypeConfiguration : BaseEntityTypeConfiguration<CustomerUser>
    {

        protected override void Configure(EntityTypeBuilder<CustomerUser> builder)
        {

            builder.ToTable("CustomerUsers", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(configuration =>
            {
                configuration.HistorySchema(ApplicationUserDbContext.HISTORY_SCHEMA);
                configuration.HistoryTable("CustomerUsers");
            });

            builder.Property(user => user.Id)
                    .HasColumnName("UserId");

            builder.Property(o => o.CountryMeta)
                .HasConversion(o => o.Id, o => Enumeration.FindById<CountryMeta>(o))
                .HasColumnName("CountryCode");

            builder.Property(e => e.IsTPNUser)
               .IsRequired(true)
               .HasDefaultValue(0);


      
        }
    }
}