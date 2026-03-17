using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class EmailRecipientEntityTypeConfiguration : BaseEntityTypeConfiguration<EmailRecipient>
    {
        protected override void Configure(EntityTypeBuilder<EmailRecipient> builder)
        {
            builder.ToTable("EmailRecipients", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("EmailRecipients");
            });


            //Primary Key
            builder.Property(kyc => kyc.Id)
                    .HasColumnName("EmailRecipientId");
            builder.HasKey(kyc => kyc.Id);

            builder.HasOne(o => o.NotificationTemplate)
                .WithMany()
                .HasForeignKey("NotificationTemplateCode");

            builder.HasOne(o => o.RecipientType)
                .WithMany()
                .HasForeignKey("RecipientTypeCode");

            builder.HasOne(o => o.CollectionTier)
                .WithMany()
                .HasForeignKey("CollectionTierCode")
                .IsRequired(false);

            builder.HasOne(o=>o.AuthorityLevel)
                .WithMany()
                .HasForeignKey("AuthorityLevelCode")
                .IsRequired(false);
        }
    }
}
