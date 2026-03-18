using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class AccountStatusEntityTypeConfiguration : BaseEntityTypeConfiguration<AccountStatus>
    {
        protected override void Configure(EntityTypeBuilder<AccountStatus> builder)
        {            
            builder.ToTable("AccountStatus", ApplicationUserDbContext.META_SCHEMA);
            
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("AccountStatusCode");

            builder.Property(o => o.Name)
                .HasMaxLength(20)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<AccountStatus>());
        }
    }
}
