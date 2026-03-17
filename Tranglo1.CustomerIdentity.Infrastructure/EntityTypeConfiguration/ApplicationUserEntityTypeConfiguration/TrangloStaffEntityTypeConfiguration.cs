using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class TrangloStaffEntityTypeConfiguration : BaseEntityTypeConfiguration<TrangloStaff>
    {
        protected override void Configure(EntityTypeBuilder<TrangloStaff> builder)
        {
            builder.ToTable("TrangloStaffs", ApplicationUserDbContext.DEFAULT_SCHEMA); 
            
            builder.Property(user => user.Id)
                     .HasColumnName("UserId");

            builder.HasMany(b => b.TrangloStaffAssignments)
                .WithOne()
                .HasForeignKey("UserId")
                .IsRequired(true);

            builder.HasMany(b => b.TrangloStaffEntityAssignments)
                .WithOne()
                .HasForeignKey("UserId")
                .IsRequired(true);

            //builder.Ignore(c => c.Roles);
            var navigation = builder.Metadata.FindNavigation(nameof(TrangloStaff.TrangloStaffAssignments));
            var navigationTSEA = builder.Metadata.FindNavigation(nameof(TrangloStaff.TrangloStaffEntityAssignments));

            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the Departments collection property through its field
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            navigationTSEA.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
