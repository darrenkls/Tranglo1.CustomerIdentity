using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.CustomerUserVerification;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class JumioGetWorkflowDetailEntityTypeConfiguration : BaseEntityTypeConfiguration<JumioGetWorkflowDetail>
    {
        protected override void Configure(EntityTypeBuilder<JumioGetWorkflowDetail> builder)
        {
            builder.ToTable("JumioGetWorkflowDetails", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("JumioGetWorkflowDetails");
            });

            builder.Property(a => a.Id)
               .HasColumnName("JumioGetWorkflowDetailCode");
            builder.HasKey(a => a.Id);
        }
    }
}
