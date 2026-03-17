using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.CustomerUserVerification;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class JumioAccountCreationEntityTypeConfiguration : BaseEntityTypeConfiguration<JumioAccountCreation>
    {
        protected override void Configure(EntityTypeBuilder<JumioAccountCreation> builder)
        {
            builder.ToTable("JumioAccountCreations", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("JumioAccountCreations");
            });

            builder.Property(a => a.Id)
               .HasColumnName("JumioAccountCreationCode");
            builder.HasKey(a => a.Id);
        }
    }
}