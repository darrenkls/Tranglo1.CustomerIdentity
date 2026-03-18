using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.TransactionEvaluation;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class TransactionEvaluationQuestionInputTypeEntityTypeConfiguration : BaseEntityTypeConfiguration<TransactionEvaluationQuestionInputType>
    {
        protected override void Configure(EntityTypeBuilder<TransactionEvaluationQuestionInputType> builder)
        {
            builder.ToTable("TransactionEvaluationQuestionInputTypes", BusinessProfileDbContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("TransactionEvaluationQuestionInputTypeCode");

            builder.Property(o => o.Name)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasData(Enumeration.GetAll<TransactionEvaluationQuestionInputType>());
        }
    }
}
