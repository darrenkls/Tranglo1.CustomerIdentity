using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class SignedPartnerAgreementEntityTypeConfiguration: BaseEntityTypeConfiguration<SignedPartnerAgreement>
    {
        protected override void Configure(EntityTypeBuilder<SignedPartnerAgreement> builder)
        {
            builder.ToTable("SignedPartnerAgreement", PartnerDBContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("SignedPartnerAgreementId");

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(PartnerDBContext.HISTORY_SCHEMA);
                config.HistoryTable("SignedPartnerAgreement");
            });

            builder.HasOne(a => a.PartnerRegistration)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey("PartnerCode");

        }
    }
}
