using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class InvitationEntityTypeConfiguration : BaseEntityTypeConfiguration<Invitation>
    {
        protected override void Configure(EntityTypeBuilder<Invitation> builder)
        {
            builder.ToTable("Invitations", ApplicationUserDbContext.DEFAULT_SCHEMA);

            //Primary Key
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                   .IsRequired()
                   .HasColumnName("InvitationCode");

            builder.HasOne(o => o.BusinessProfile)
                            .WithMany()
                            .HasForeignKey("BusinessProfileCode")
                            .IsRequired(false);

            builder.OwnsOne<FullName>(user => user.FullName, fullName =>
            {
                fullName.Property(e => e.Value)
                    .HasColumnName("FullName")
                    .HasMaxLength(300);

                fullName.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            });

            builder.OwnsOne<Email>(user => user.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(320);

                email.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            });

            builder.Ignore(user => user.DomainEvents);

        }
    }
}
