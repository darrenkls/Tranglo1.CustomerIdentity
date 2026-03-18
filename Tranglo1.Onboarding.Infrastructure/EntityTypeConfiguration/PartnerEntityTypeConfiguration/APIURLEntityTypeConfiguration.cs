using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.PartnerEntityTypeConfiguration
{
    class APIURLEntityTypeConfiguration : BaseEntityTypeConfiguration<APIURL>
    {
        protected override void Configure(EntityTypeBuilder<APIURL> builder)
        {
            builder.ToTable("APIURLs", PartnerDBContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("APIURLId");
        }
    }
}
