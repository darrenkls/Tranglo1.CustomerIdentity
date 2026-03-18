using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.RBAAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class EvaluationRulesEntityTypeConfiguration : BaseEntityTypeConfiguration<EvaluationRules>
    {
        protected override void Configure(EntityTypeBuilder<EvaluationRules> builder)
        {
            builder.ToTable("EvaluationRules", RBADBContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(RBADBContext.HISTORY_SCHEMA);
                config.HistoryTable("EvaluationRules");
            });

            // Primary Key
            builder.Property(er => er.Id)
                .HasColumnName("EvaluationRulesCode");
            builder.HasKey(er => er.Id);

            // Relationships
            builder.HasOne(er => er.RBA)
                   .WithMany(rba => rba.EvaluationRules)
                   .HasForeignKey("RBACode");
        }
    }
}
