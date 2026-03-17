using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class ApplicationUserEntityTypeConfiguration : BaseEntityTypeConfiguration<ApplicationUser>
	{
		protected override void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            
            builder.ToTable("ApplicationUsers");

            builder.Property(user => user.Id)
                    .HasColumnName("UserId");
            builder.HasKey(user => user.Id);

            builder.OwnsOne<Email>(user => user.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(320);
                  
                email.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            });

            builder.OwnsOne<ContactNumber>(user => user.ContactNumber, contactNumber =>
            {
                contactNumber.Property(e => e.Value)
                    .HasColumnName("ContactNumber")
                    .HasMaxLength(15);

                contactNumber.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            });

            builder.OwnsOne<FullName>(user => user.FullName, fullName =>
            {
                fullName.Property(e => e.Value)
                    .HasColumnName("FullName")
                    .HasMaxLength(300);

                fullName.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            });

            builder.HasOne(o => o.AccountStatus)
                .WithMany()
                .HasForeignKey("AccountStatusCode")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(e => e.LoginId)
                .HasMaxLength(256)
                .IsUnicode(true)
                .IsRequired(true);

            builder.Property(e => e.Timezone)
               .HasMaxLength(256)
               .IsRequired(false);

            builder.HasIndex(e => e.LoginId).IsUnique(true).HasDatabaseName("UNIQ_ApplicationUsers_LoginId");
                        
            builder.Ignore(user => user.DomainEvents);

        }
    }
}
