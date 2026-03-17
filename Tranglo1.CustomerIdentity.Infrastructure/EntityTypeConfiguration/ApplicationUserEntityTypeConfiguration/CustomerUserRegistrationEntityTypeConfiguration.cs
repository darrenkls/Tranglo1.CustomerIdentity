using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class CustomerUserRegistrationEntityTypeConfiguration : BaseEntityTypeConfiguration<CustomerUserRegistration>
    {
        protected override void Configure(EntityTypeBuilder<CustomerUserRegistration> builder)
        {

            builder.ToTable("CustomerUserRegistrations", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.Property(user => user.LoginId)
                .HasMaxLength(150)
                .HasColumnName("LoginId");

            builder.HasKey(user => user.CustomerUserRegistrationId);
            builder.Property(user => user.CustomerUserRegistrationId)
                .HasColumnName("CustomerUserRegistrationId");

            builder.Property(user => user.CompanyName)
                .HasMaxLength(150)
                .HasColumnName("CompanyName")
                .IsRequired(false);

            builder.Property(user => user.SignUpCode)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(user => user.BusinessProfileCode)
                .HasColumnName("BusinessProfileCode")
                .IsRequired(true);

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
