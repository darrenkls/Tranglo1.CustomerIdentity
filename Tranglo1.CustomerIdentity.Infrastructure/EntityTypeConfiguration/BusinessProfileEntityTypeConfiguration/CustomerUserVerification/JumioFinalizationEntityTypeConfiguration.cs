using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.CustomerUserVerification;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class JumioFinalizationEntityTypeConfiguration : BaseEntityTypeConfiguration<JumioFinalization>
    {
        protected override void Configure(EntityTypeBuilder<JumioFinalization> builder)
        {
            builder.ToTable("JumioFinalizations", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("JumioFinalizations");
            });

            builder.Property(a => a.Id)
               .HasColumnName("JumioFinalizationCode");
            builder.HasKey(a => a.Id);
        }
    }
}
