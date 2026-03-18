using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class SystemEnvironmentEntityTypeConfiguration : BaseEntityTypeConfiguration<SystemEnvironment>
    {
        protected override void Configure(EntityTypeBuilder<SystemEnvironment> builder)
        {
            builder.ToTable("SystemEnvironments", ApplicationUserDbContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("SystemEnvironmentCode");

            builder.Property(o => o.Name)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<SystemEnvironment>());
        }
    }
}
