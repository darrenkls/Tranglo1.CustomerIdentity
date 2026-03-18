using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class DocumentReleaseBPEntityTypeConfiguration : BaseEntityTypeConfiguration<DocumentReleaseBP>
    {
        protected override void Configure(EntityTypeBuilder<DocumentReleaseBP> builder)
        {
            builder.ToTable("DocumentReleaseBPs", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("DocumentReleaseBPs");
            });

            //Primary Key
            builder.Property(kyc => kyc.Id)
                    .HasColumnName("DocumentReleaseBPCode");
            builder.HasKey(kyc => kyc.Id);

            builder.HasOne(o => o.BusinessProfile)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey("BusinessProfileCode");
        }
    }
}
