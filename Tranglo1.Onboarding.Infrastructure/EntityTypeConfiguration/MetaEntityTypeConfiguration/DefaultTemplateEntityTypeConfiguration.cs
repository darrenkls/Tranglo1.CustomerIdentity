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
    class DefaultTemplateEntityTypeConfiguration : BaseEntityTypeConfiguration<DefaultTemplate>
    {
        protected override void Configure(EntityTypeBuilder<DefaultTemplate> builder)
        {
            builder.ToTable("DefaultTemplates", BusinessProfileDbContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
               .IsRequired()
               .HasColumnName("DefaultTemplateCode");

            builder.Property(o => o.Name)
                .HasMaxLength(500)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<DefaultTemplate>());
        }
    }
}