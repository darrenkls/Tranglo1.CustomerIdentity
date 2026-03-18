using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class KYCSubCategoriesEntityTypeConfiguration : BaseEntityTypeConfiguration<KYCSubCategory>
    {
        protected override void Configure(EntityTypeBuilder<KYCSubCategory> builder)
        {
            builder.ToTable("KYCSubCategories", BusinessProfileDbContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("KYCSubCategoryCode");

            builder.Property(o => o.Name)
                .HasMaxLength(300)
                .HasColumnName("Description");

            builder.Property(o => o.KycCategory)
                .IsRequired(false)
                .HasColumnName("KYCCategoryCode");


            builder.HasData(Enumeration.GetAll<KYCSubCategory>());

        }
    }
}
