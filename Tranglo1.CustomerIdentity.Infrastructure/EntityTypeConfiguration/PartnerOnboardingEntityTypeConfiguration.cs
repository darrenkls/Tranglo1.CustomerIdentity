using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Events;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class PartnerOnboardingEntityTypeConfiguration : BaseEntityTypeConfiguration<PartnerProfileChangedEvent>
    {
        protected override void Configure(EntityTypeBuilder<PartnerProfileChangedEvent> builder)
        {
            builder.ToTable("PartnerProfileChangedEvent", PartnerDBContext.EVENTS_SCHEMA);

            //builder.HasTemporalTable(config =>
            //{
            //    config.HistorySchema(PartnerDBContext.HISTORY_SCHEMA);
            //    config.HistoryTable("PartnerOnboardingGoLive");
            //});

            //Primary Key
            builder.Property(x => x.EventId)
                      .HasColumnName("EventId");
            builder.HasKey(x => new { x.EventId });

            builder.Property(x => x.SettlementCurrencyCode)
                .IsRequired(true);

            builder.OwnsOne<ContactNumber>(x => x.TelephoneNumber, x =>
            {
                x.Property(e => e.Value)
                    .HasColumnName("ContactNumber")
                    .HasMaxLength(50);

                x.Property(e => e.DialCode)
                    .HasColumnName("DialCode")
                    .HasMaxLength(50);

                x.Property(e => e.CountryISO2Code)
                    .HasColumnName("ContactNumberCountryISO2Code")
                    .HasMaxLength(2);

                x.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            });

            builder.OwnsOne<Email>(user => user.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(320);

                email.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            });

            builder.Property(e => e.CompanyRegistrationNo)
                    .HasMaxLength(150);

            builder.Property(e => e.CompanyName)
                    .HasMaxLength(150);

            builder.Property(e => e.CompanyRegisteredAddress)
                    .HasMaxLength(500);

            builder.Property(e => e.CompanyRegisteredZipCodePostCode)
                    .HasMaxLength(100);

            builder.Property(e => e.BusinessNatureCode)
                    .HasMaxLength(5)
                    .IsRequired(false);

            builder.Property(e => e.BusinessNatureDescription)
                    .HasMaxLength(300)
                    .IsRequired(false);
        }
    }
}
