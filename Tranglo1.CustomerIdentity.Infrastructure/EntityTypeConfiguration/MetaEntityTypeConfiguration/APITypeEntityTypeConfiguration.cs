using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.MetaEntityTypeConfiguration
{
    class APITypeEntityTypeConfiguration : BaseEntityTypeConfiguration<APIType>
    {
        protected override void Configure(EntityTypeBuilder<APIType> builder)
        {
            builder.ToTable("APIType", ApplicationUserDbContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("APITypeCode");

            builder.Property(o => o.Name)
                .HasMaxLength(20)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<APIType>());
        }
    }
}
