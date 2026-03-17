using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{


    class TrangloStaffEntityAssignmentEntityTypeConfiguration : BaseEntityTypeConfiguration<TrangloStaffEntityAssignment>
    {
        protected override void Configure(EntityTypeBuilder<TrangloStaffEntityAssignment> builder)
        {

            builder.ToTable("TrangloStaffEntityAssignments", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(ApplicationUserDbContext.HISTORY_SCHEMA);
                config.HistoryTable("TrangloStaffEntityAssignments");
            });        

            builder.HasOne(o => o.BlockStatus)
                .WithMany()
                .HasForeignKey("BlockStatusCode")
                .IsRequired(true);

            builder.HasOne(o => o.AccountStatus)
                .WithMany()
                .HasForeignKey("AccountStatusCode")
                .IsRequired(true);

            builder.HasKey(assignment => new {
                assignment.LoginId,
                assignment.TrangloEntity
            });

        }
    }

}
