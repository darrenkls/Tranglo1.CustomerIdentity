using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.OTP;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.OTPEntityTypeConfiguration
{
    class RequisitionOTPEntityTypeConfiguration : BaseEntityTypeConfiguration<RequisitionOTP>
    {
        protected override void Configure(EntityTypeBuilder<RequisitionOTP> builder)
        {
            builder.ToTable("RequisitionOTPs", OTPDbContext.DEFAULT_SCHEMA);

            builder.Property(kyc => kyc.Id)
                   .HasColumnName("RequisitionOTPId");
            builder.HasKey(kyc => kyc.Id);

        }
    }
}
