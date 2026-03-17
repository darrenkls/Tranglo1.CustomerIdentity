using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class PartnerAgreementEntityTypeConfiguration: BaseEntityTypeConfiguration<PartnerAgreementTemplate>
    {
        protected override void Configure(EntityTypeBuilder<PartnerAgreementTemplate> builder)
        {
            builder.ToTable("PartnerAgreementTemplate", PartnerDBContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("PartnerAgreementTemplateId");

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(PartnerDBContext.HISTORY_SCHEMA);
                config.HistoryTable("PartnerAgreementTemplate");
            });

            builder.HasOne(a => a.PartnerRegistration)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey("PartnerCode");

        }
    }
}