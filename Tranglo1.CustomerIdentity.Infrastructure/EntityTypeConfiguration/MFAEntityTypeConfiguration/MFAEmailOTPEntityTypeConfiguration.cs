using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Domain.Entities.OTP;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.MFAEntityTypeConfiguration
{
    internal class MFAEmailOTPEntityTypeConfiguration : BaseEntityTypeConfiguration<MFAEmailOTP>
    {
        protected override void Configure(EntityTypeBuilder<MFAEmailOTP> builder)
        {
            builder.ToTable("MFAEmailOTPs", OTPDbContext.DEFAULT_SCHEMA);

            builder.Property(x => x.Id)
                   .HasColumnName("MFAEmailOTPId");
            builder.HasKey(x => x.Id);
        }
    }
}
