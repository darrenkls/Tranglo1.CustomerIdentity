using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.MetaEntityTypeConfiguration
{
    class BusinessIDTypeEntityTypeConfiguration : BaseEntityTypeConfiguration<BusinessProfileIDType>
    {
        protected override void Configure(EntityTypeBuilder<BusinessProfileIDType> builder)
        {
            builder.ToTable("BusinessProfileIDTypes", BusinessProfileDbContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("BusinessProfileIDTypeCode");

            builder.Property(o => o.Name)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<BusinessProfileIDType>());
        }
    }
}