using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class DocumentCategoryBPEntityTypeConfiguration : BaseEntityTypeConfiguration<DocumentCategoryBP>
    {
        protected override void Configure(EntityTypeBuilder<DocumentCategoryBP> builder)
        {
            builder.ToTable("DocumentCategoryBPs", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("DocumentCategoryBPs");
            });

            //Primary Key
            builder.Property(kyc => kyc.Id)
                    .HasColumnName("DocumentCategoryBPCode");
            builder.HasKey(kyc => kyc.Id);

            builder.HasOne(o => o.DocumentCategory)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey("DocumentCategoryCode");

            builder.HasOne(o => o.BusinessProfile)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey("BusinessProfileCode");

            builder.HasOne(o => o.DocumentCategoryBPStatus)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey("DocumentCategoryBPStatusCode");




        }
    }
}
