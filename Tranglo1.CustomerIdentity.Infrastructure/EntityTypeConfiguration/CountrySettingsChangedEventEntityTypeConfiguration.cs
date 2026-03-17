using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Events;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class CountrySettingsChangedEventEntityTypeConfiguration : BaseEntityTypeConfiguration<CountrySettingsChangedEvent>
    {
        protected override void Configure(EntityTypeBuilder<CountrySettingsChangedEvent> builder)
        {
            builder.ToTable("CountrySettingsChangedEvent", PartnerDBContext.EVENTS_SCHEMA);

            //builder.HasTemporalTable(config =>
            //{
            //    config.HistorySchema(PartnerDBContext.HISTORY_SCHEMA);
            //    config.HistoryTable("PartnerOnboardingGoLive");
            //});

            //Primary Key
            builder.Property(x => x.EventId)
                      .HasColumnName("EventId");
            builder.HasKey(x => new { x.EventId });

            builder.Property(o => o.IsHighRisk)
               .HasColumnName("IsHighRisk");

            builder.Property(o => o.IsSanction)
                .HasColumnName("IsSanction");

            builder.Property(o => o.IsDisplay)
                .HasColumnName("IsDisplay");

            builder.Property(o => o.IsRejectTransaction)
                .HasColumnName("IsRejectTransaction");

            builder.Property(o => o.CountryCode)
                .HasColumnName("CountryCode");

            builder.Property(o => o.CountryISO2)
                .HasColumnName("CountryISO2");
        }
    }
}
