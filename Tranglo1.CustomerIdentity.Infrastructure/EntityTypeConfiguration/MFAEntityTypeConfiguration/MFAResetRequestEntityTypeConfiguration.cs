using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.MFAEntityTypeConfiguration
{
    internal class MFAResetRequestEntityTypeConfiguration : BaseEntityTypeConfiguration<MFAResetRequest>
    {
        protected override void Configure(EntityTypeBuilder<MFAResetRequest> builder)
        {
            builder.ToTable("MFAResetRequests", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("MFAResetRequestId");

            builder.Property(o => o.UserId)
                .IsRequired();

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            builder.Property(o => o.Token)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
