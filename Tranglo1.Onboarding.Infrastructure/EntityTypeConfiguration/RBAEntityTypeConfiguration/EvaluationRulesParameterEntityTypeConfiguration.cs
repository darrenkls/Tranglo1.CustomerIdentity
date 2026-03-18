using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.RBAAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.RBAEntityTypeConfiguration
{
    class EvaluationRulesParameterEntityTypeConfiguration : BaseEntityTypeConfiguration<EvaluationRulesParameter>
    {
        protected override void Configure(EntityTypeBuilder<EvaluationRulesParameter> builder)
        {
            builder.ToTable("EvaluationRulesParameter", RBADBContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(RBADBContext.HISTORY_SCHEMA);
                config.HistoryTable("EvaluationRulesParameter");
            });

            // Primary Key
            builder.Property(er => er.Id)
                .HasColumnName("EvaluationRuleParameterCode");
            builder.HasKey(er => er.Id);
        }
    }
}
