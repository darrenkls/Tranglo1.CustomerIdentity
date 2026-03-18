using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.BusinessDeclaration;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class BusinessDeclarationRejectionMatrixEntityTypeConfiguration : BaseEntityTypeConfiguration<BusinessDeclarationRejectionMatrix>
    {
        protected override void Configure(EntityTypeBuilder<BusinessDeclarationRejectionMatrix> builder)
        {
            builder.ToTable("BusinessDeclarationRejectionMatrixes", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("BusinessDeclarationRejectionMatrixes");
            });

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("BusinessDeclarationRejectionMatrixCode");
        }
    }
}