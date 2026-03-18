using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class DefaultTemplateDocumentEntityTypeConfiguration : BaseEntityTypeConfiguration<DefaultTemplateDocument>
    {
        protected override void Configure(EntityTypeBuilder<DefaultTemplateDocument> builder)
        {
            builder.ToTable("DefaultTemplateDocuments", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("DefaultTemplateDocuments");
            });

            builder.Property(o => o.Id)
               .IsRequired()
               .HasColumnName("DefaultTemplateDocumentCode");

            builder.HasOne(o => o.DefaultTemplate)
                .WithMany()
                .HasForeignKey("DefaultTemplateCode")
                .IsRequired(false);
        }
    }
}