using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    
    class CompanyShareholderEntityTypeConfiguration : BaseEntityTypeConfiguration<CompanyShareholder>
    {
        protected override void Configure(EntityTypeBuilder<CompanyShareholder> builder)
        {
            builder.ToTable("CompanyShareholders", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("CompanyShareholders");
            });

            builder.Property(e => e.CompanyName)
                    .HasMaxLength(150);

            builder.Property(e => e.CompanyRegNo)
                    .HasMaxLength(150);

            builder.HasOne(o => o.Country)
              .WithMany()
              .HasForeignKey("CountryCode")
              .IsRequired(false);
        }
    }
    
}
