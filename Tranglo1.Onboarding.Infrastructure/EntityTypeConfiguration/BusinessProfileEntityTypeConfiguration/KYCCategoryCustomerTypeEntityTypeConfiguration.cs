using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class KYCCategoryCustomerTypeEntityTypeConfiguration : BaseEntityTypeConfiguration<KYCCategoryCustomerType>
    {
        protected override void Configure(EntityTypeBuilder<KYCCategoryCustomerType> builder)
        {
            builder.ToTable("KYCCategoryCustomerTypes", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("KYCCategoryCustomerTypeCode");

            builder.HasOne(o => o.KYCCategory)
               .WithMany()
               .IsRequired(true)
               .HasForeignKey("KYCCategoryCode");
        }
    }
}