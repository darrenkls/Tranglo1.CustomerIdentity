using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.MFAEntityTypeConfiguration
{
    internal class MFAEntityTypeConfiguration : BaseEntityTypeConfiguration<MFA>
    {
        protected override void Configure(EntityTypeBuilder<MFA> builder)
        {
            builder.ToTable("MultiFactorAuthentications", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(ApplicationUserDbContext.HISTORY_SCHEMA);
                config.HistoryTable("MultiFactorAuthentications");
            });

            builder.Property(o => o.Id).HasColumnName("MFAId");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Token).HasColumnName("Token");
            builder.Property(o => o.RecoveryCode).HasColumnName("RecoveryCode");

            builder.Property(o => o.UserId);
            builder.Property(o => o.AuthenticationType)
                  .HasConversion(o => o.Id, o => Enumeration.FindById<AuthenticationType>(o))
                  .HasColumnName("AuthenticationTypeCode");



        }
    }
}
