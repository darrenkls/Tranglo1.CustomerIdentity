using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class TrangloStaffAssignmentEntityTypeConfiguration : BaseEntityTypeConfiguration<TrangloStaffAssignment>
    {
        protected override void Configure(EntityTypeBuilder<TrangloStaffAssignment> builder)
        {
            builder.ToTable("TrangloStaffAssignments", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(ApplicationUserDbContext.HISTORY_SCHEMA);
                config.HistoryTable("TrangloStaffAssignments");
            });


            builder.Property(o => o.TrangloEntity)
                 .IsRequired()
                 .HasMaxLength(300);

            builder.Property(o => o.LoginId)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasOne(o => o.TrangloDepartment)
                .WithMany()
                .IsRequired()
                .HasForeignKey("TrangloDepartmentCode");

            builder.Property(o => o.RoleCode)
                .IsRequired();

            //Primary Key
            builder.HasKey(assignment => new {
                assignment.LoginId,
                assignment.TrangloEntity,
                assignment.TrangloDepartmentCode,
                assignment.RoleCode
            });

            /*
            builder.ToTable("TrangloStaffAssignments", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(ApplicationUserDbContext.HISTORY_SCHEMA);
                config.HistoryTable("TrangloStaffAssignments");
            });

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("TrangloStaffAssignmentCode");

            builder.HasOne(o => o.TrangloStaffEntityAssignment)
           .WithMany()
           .HasForeignKey("TrangloStaffEntityAssignmentCode")
           .IsRequired(true);


            builder.Property(o => o.LoginId)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasOne(o => o.TrangloDepartment)
                .WithMany()
                .HasForeignKey("TrangloDepartmentCode")
                .IsRequired(true);
            builder.HasOne(o => o.TrangloRole)
              .WithMany()
              .HasForeignKey("TrangloRoleCode")
              .IsRequired(true);
            */

        }
    }
}
