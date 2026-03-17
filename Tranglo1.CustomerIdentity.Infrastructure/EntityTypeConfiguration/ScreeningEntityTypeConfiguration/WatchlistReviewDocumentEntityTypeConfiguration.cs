using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class WatchlistReviewDocumentEntityTypeConfiguration : BaseEntityTypeConfiguration<WatchlistReviewDocument>
    {
        protected override void Configure(EntityTypeBuilder<WatchlistReviewDocument> builder)
        {
            builder.ToTable("WatchlistReviewDocuments", ScreeningDBContext.DEFAULT_SCHEMA);

            //Primary Key
            builder.Property(x => x.Id)
                    .HasColumnName("WatchlistReviewDocumentCode");
            builder.HasKey(x => x.Id);

            builder.HasOne(o => o.WatchlistReview)
                .WithMany()
                .HasForeignKey("WatchlistReviewCode")
                .IsRequired(true);

        }
    }
}
