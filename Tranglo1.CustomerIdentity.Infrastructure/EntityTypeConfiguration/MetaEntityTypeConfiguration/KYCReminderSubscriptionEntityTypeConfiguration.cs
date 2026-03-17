using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.MetaEntityTypeConfiguration
{
     class KYCReminderSubscriptionEntityTypeConfiguration : BaseEntityTypeConfiguration<KYCReminderSubscription>
    {
        protected override void Configure(EntityTypeBuilder<KYCReminderSubscription> builder)
        {
            builder.ToTable("KYCReminderSubscriptions", ApplicationUserDbContext.META_SCHEMA);
            {
                builder.HasKey(o => o.Id);

                builder.Property(o => o.Id)
                    .IsRequired()
                    .HasColumnName("KYCReminderSubscriptionCode");

                builder.Property(o => o.Name)
                    .HasColumnName("Description");

                builder.HasData(Enumeration.GetAll<KYCReminderSubscription>());
            }
        }
    }
}
