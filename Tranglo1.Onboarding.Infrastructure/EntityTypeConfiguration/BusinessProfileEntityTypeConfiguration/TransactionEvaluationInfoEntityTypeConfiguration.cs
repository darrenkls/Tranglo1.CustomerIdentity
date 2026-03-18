using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.BusinessDeclaration;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.TransactionEvaluation;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class TransactionEvaluationInfoEntityTypeConfiguration : BaseEntityTypeConfiguration<TransactionEvaluationInfo>
    {
        protected override void Configure(EntityTypeBuilder<TransactionEvaluationInfo> builder)
        {
            builder.ToTable("TransactionEvaluationInfos", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("TransactionEvaluationInfos");
            });

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("TransactionEvaluationInfoCode");

            builder.Property(o => o.TransactionEvaluationInfoDescription)
                 .HasMaxLength(500);

        }
    }
}
