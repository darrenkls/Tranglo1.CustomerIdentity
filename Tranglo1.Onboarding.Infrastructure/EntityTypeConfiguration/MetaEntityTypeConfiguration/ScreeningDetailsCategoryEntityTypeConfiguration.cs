using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities.ScreeningAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.MetaEntityTypeConfiguration
{
    class ScreeningDetailsCategoryEntityTypeConfiguration : IEntityTypeConfiguration<ScreeningDetailsCategory>
    {
        public void Configure(EntityTypeBuilder<ScreeningDetailsCategory> builder)
        {
            builder.ToTable("ScreeningDetailsCategory", ScreeningDBContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("ScreeningDetailsCategoryCode");

            builder.Property(o => o.Name)
                .HasMaxLength(150)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<ScreeningDetailsCategory>());
        }
    }
}
