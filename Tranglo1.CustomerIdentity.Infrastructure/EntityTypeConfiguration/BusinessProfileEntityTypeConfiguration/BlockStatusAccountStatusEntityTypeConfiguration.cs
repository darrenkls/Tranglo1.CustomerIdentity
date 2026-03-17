using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class BlockStatusAccountStatusEntityTypeConfiguration : IEntityTypeConfiguration<CustomerUserBusinessProfile>
    {
        public void Configure(EntityTypeBuilder<CustomerUserBusinessProfile> builder)
        {
            builder.ToTable("CustomerUserBusinessProfiles", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasOne(o => o.CompanyUserAccountStatus)
                .WithMany()
                .HasForeignKey("CompanyUserAccountStatusCode")
                .IsRequired(false);

            builder.HasOne(o => o.CompanyUserBlockStatus)
                .WithMany()
                .HasForeignKey("CompanyUserBlockStatusCode")
                .IsRequired(false);

        }
    }
}
