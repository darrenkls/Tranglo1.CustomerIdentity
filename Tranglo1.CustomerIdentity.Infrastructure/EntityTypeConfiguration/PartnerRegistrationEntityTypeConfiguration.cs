using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class PartnerRegistrationEntityTypeConfiguration : BaseEntityTypeConfiguration<PartnerRegistration>
    {
        protected override void Configure(EntityTypeBuilder<PartnerRegistration> builder)
        {
            builder.ToTable("PartnerRegistrations", PartnerDBContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(PartnerDBContext.HISTORY_SCHEMA);
                config.HistoryTable("PartnerRegistrations");
            });

            builder.Property(a => a.Id)
                .HasColumnName("PartnerCode");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.IMID)
                    .HasMaxLength(150);

            builder.Property(a => a.IMID)
                    .HasMaxLength(150);

            builder.Property(a => a.TrangloEntity)
                    .HasMaxLength(150);
            /*
            builder.Property(a => a.CurrencyCode)
                   .HasMaxLength(150);
            */
            builder.OwnsOne<Email>(user => user.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(320);

                email.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            });

            builder.HasOne(a => a.PartnerType)
                .WithMany()
                .HasForeignKey("PartnerTypeCode")
                .IsRequired(false);

            builder.HasOne(pr => pr.BusinessProfile)
                .WithMany(bp => bp.PartnerRegistrations)
                .HasForeignKey(x => x.BusinessProfileCode)
                .IsRequired(true);

            builder.Property(a => a.TimeZone)
               .HasMaxLength(256)
               .IsRequired(false);

            builder.Property(a => a.PartnerId)
                .IsRequired(false);

            builder.Property(x => x.AgentLoginId)
                    .HasColumnName("AgentLoginId")
                   .HasMaxLength(256)
                   .IsRequired(false);

            builder.Property(x => x.ProductLoginId)
                    .HasColumnName("ProductLoginId")
                   .HasMaxLength(256)
                   .IsRequired(false);

            builder.Property(x => x.SalesOperationLoginId)
                    .HasColumnName("SalesOperationLoginId")
                   .HasMaxLength(256)
                   .IsRequired(false);
            //builder.HasKey(AgentLoginId => AgentLoginId.Id);

            builder.Ignore(o => o.DomainEvents);

            builder.HasOne(a => a.PartnerAccountStatusType)
                .WithMany()
                .HasForeignKey("PartnerAccountStatusTypeCode")
                .IsRequired(false);

            builder.HasMany(b => b.PartnerAccountStatuses)
               .WithOne()
               .HasForeignKey("PartnerCode")
               .IsRequired(false);

            builder.HasOne(o => o.AgreementOnboardWorkflowStatus)
            .WithMany()
            .HasForeignKey("AgreementOnboardWorkflowStatusCode")
            .IsRequired(false);


            builder.HasOne(o => o.APIIntegrationOnboardWorkflowStatus)
            .WithMany()
            .HasForeignKey("APIIntegrationOnboardWorkflowStatusCode")
            .IsRequired(false);

            builder.HasOne(a => a.CustomerType)
             .WithMany()
             .HasForeignKey("CustomerTypeCode")
             .IsRequired(false);

            //builder.HasOne(a => a.Environment)
            //.WithOne()
            //.HasForeignKey("EnvironmentCode")
            //.IsRequired(true);

            //builder.Property(a => a.Environment)
            //.HasDefaultValue(false);

            var navigation = builder.Metadata.FindNavigation(nameof(PartnerRegistration.PartnerAccountStatuses));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            /* builder.HasOne(o => o.Country)
                 .WithMany()
                 .HasForeignKey("CountryISO2")
                 .IsRequired(false);*/

            builder.HasOne(user => user.PartnerRegistrationLeadsOrigin)
                .WithMany()
                .HasForeignKey("PartnerRegistrationLeadsOriginCode")
                .IsRequired(false);

            builder.Property(user => user.OtherPartnerRegistrationLeadsOrigin)
                .HasMaxLength(100)
                .IsRequired(false);
        }
    }
}
