using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class SignUpCodeEntityTypeConfiguration : BaseEntityTypeConfiguration<SignUpCode>
    {
        protected override void Configure(EntityTypeBuilder<SignUpCode> builder)
        {
            builder.ToTable("SignUpCodes", SignUpCodeDBContext.DEFAULT_SCHEMA);
          
            builder.Property(o => o.Id)
                      .HasColumnName("SignUpCode");
            builder.HasKey(o => o.Id);
           
            builder.HasOne(o => o.Status)
              .WithMany()
              .HasForeignKey("SignUpCodeStatusCode");

            builder.HasOne(o => o.LeadsOrigin)
                .WithMany()
                .HasForeignKey("LeadsOriginCode");

            builder.Property(o => o.CompanyName)
                   .HasMaxLength(150);

            builder.Ignore(o => o.DomainEvents);
        }
    }
}
