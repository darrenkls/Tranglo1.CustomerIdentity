using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tranglo1.CustomerIdentity.Domain.Entities;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class DocumentCommentUploadBPEntityTypeConfiguration : BaseEntityTypeConfiguration<DocumentCommentUploadBP>
    {
        protected override void Configure(EntityTypeBuilder<DocumentCommentUploadBP> builder)
        {
            builder.ToTable("DocumentCommentUploadBPs", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("DocumentCommentUploadBPs");

            });

            builder.HasOne(o => o.DocumentCommentBP)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey("DocumentCommentBPCode")
                .OnDelete(DeleteBehavior.Cascade);

            //Primary Key
            builder.HasKey(kyc => new { kyc.DocumentCommentBPCode, kyc.DocumentId });
        }
    }
}
