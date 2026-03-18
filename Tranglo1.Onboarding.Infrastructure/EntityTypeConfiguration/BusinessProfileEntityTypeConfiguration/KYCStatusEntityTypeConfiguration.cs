using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class KYCStatusEntityTypeConfiguration : BaseEntityTypeConfiguration<KYCStatus>
    {
        protected override void Configure(EntityTypeBuilder<KYCStatus> builder)
        {
            builder.ToTable("KYCStatuses", BusinessProfileDbContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("KYCStatusCode");

            builder.Property(o => o.Name)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<KYCStatus>());
        }
    }
}
