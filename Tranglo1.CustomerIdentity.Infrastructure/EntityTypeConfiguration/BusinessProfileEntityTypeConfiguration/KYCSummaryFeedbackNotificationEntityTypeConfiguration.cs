using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class KYCSummaryFeedbackNotificationEntityTypeConfiguration : BaseEntityTypeConfiguration<KYCSummaryFeedbackNotification>
    {
        protected override void Configure(EntityTypeBuilder<KYCSummaryFeedbackNotification> builder)
        {
            builder.ToTable("KYCSummaryFeedbackNotification", BusinessProfileDbContext.DEFAULT_SCHEMA);

            //Primary Key
            builder.Property(kyc => kyc.Id)
                    .HasColumnName("KYCSummaryFeedbackNotificationCode");
            builder.HasKey(kyc => kyc.Id);

            builder.HasOne(o => o.KYCSummaryFeedback)
              .WithMany()
              .IsRequired()
              .HasForeignKey("KYCSummaryFeedbackCode");

            builder.HasOne(o => o.BusinessProfile)
              .WithMany()
              .IsRequired()
              .HasForeignKey("BusinessProfileCode");

            builder.Property(o => o.Event)
                .HasMaxLength(20);
        }
    }
}
