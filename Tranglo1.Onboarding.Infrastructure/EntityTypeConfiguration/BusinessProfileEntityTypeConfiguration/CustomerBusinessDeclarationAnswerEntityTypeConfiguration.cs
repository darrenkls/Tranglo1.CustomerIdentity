using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.BusinessDeclaration;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class CustomerBusinessDeclarationAnswerEntityTypeConfiguration : BaseEntityTypeConfiguration<CustomerBusinessDeclarationAnswer>
    {
        protected override void Configure(EntityTypeBuilder<CustomerBusinessDeclarationAnswer> builder)
        {
            builder.ToTable("CustomerBusinessDeclarationAnswers", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("CustomerBusinessDeclarationAnswers");
            });

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("CustomerBusinessDeclarationAnswerCode");
        }
    }
}