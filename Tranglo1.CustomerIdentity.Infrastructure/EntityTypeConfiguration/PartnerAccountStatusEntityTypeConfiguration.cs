using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class PartnerAccountStatusEntityTypeConfiguration : BaseEntityTypeConfiguration<PartnerAccountStatus>
    
    {
        protected override void Configure(EntityTypeBuilder<PartnerAccountStatus> builder)
        {
            builder.ToTable("PartnerAccountStatus", PartnerDBContext.DEFAULT_SCHEMA);
            /*
            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(PartnerDBContext.HISTORY_SCHEMA);
                config.HistoryTable("PartnerAccountStatus");
            });
            */
            //Primary Key
            builder.Property(kyc => kyc.Id)
                      .HasColumnName("PartnerAccountStatusCode");
            builder.HasKey(kyc => kyc.Id);
            /*
            builder.HasOne(o => o.PartnerRegistration)
                .WithMany()
                .HasForeignKey("PartnerRegistrationCode");
            */
            builder.HasOne(o => o.PartnerAccountStatusType)
              .WithMany()
              .HasForeignKey("PartnerAccountStatusTypeCode");

            builder.HasOne(o => o.ChangeType)
                .WithMany()
                .HasForeignKey("ChangeTypeCode");

            builder.Property(x => x.Description)
                   .HasMaxLength(150);

            builder.Property(x => x.PartnerSubscriptionCode)
                .IsRequired(false);
        }
    }
}
