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
    class ComplianceRequisitionTypeEntityTypeConfiguration : BaseEntityTypeConfiguration<ComplianceRequisitionType>
    {
        protected override void Configure(EntityTypeBuilder<ComplianceRequisitionType> builder)
        {
            builder.ToTable("ComplianceRequisitionTypes", BusinessProfileDbContext.META_SCHEMA);

            builder.Property(x => x.Id)
                .HasColumnName("ComplianceRequisitionTypeCode");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<ComplianceRequisitionType>());
        }
    }
}