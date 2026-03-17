using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class PartnerAccountStatusTypeEntityTypeConfiguration: BaseEntityTypeConfiguration<PartnerAccountStatusType>
    
    {
        protected override void Configure(EntityTypeBuilder<PartnerAccountStatusType> builder)
        {
            builder.ToTable("PartnerAccountStatusTypes", PartnerDBContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("PartnerAccountStatusTypeCode");

            builder.Property(o => o.Name)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<PartnerAccountStatusType>());
        }
    }
}
