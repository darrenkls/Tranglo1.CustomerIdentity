using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.BusinessDeclaration;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class DeclarationQuestionEntityTypeConfiguration : BaseEntityTypeConfiguration<DeclarationQuestion>
    {
        protected override void Configure(EntityTypeBuilder<DeclarationQuestion> builder)
        {
            builder.ToTable("DeclarationQuestions", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("DeclarationQuestions");
            });

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("DeclarationQuestionCode");

            builder.HasOne(o => o.CustomerType)
              .WithMany()
              .HasForeignKey("CustomerTypeCode");

            builder.HasOne(o => o.DeclarationQuestionType)
              .WithMany()
              .HasForeignKey("DeclarationQuestionTypeCode");
        }
    }
}