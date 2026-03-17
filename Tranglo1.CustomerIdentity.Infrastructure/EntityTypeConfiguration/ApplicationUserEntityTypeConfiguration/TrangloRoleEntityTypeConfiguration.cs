using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class TrangloRoleEntityTypeConfiguration : BaseEntityTypeConfiguration<TrangloRole>
    {
        protected override void Configure(EntityTypeBuilder<TrangloRole> builder)
        {
            builder.ToTable("TrangloRoles", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(ApplicationUserDbContext.HISTORY_SCHEMA);
                config.HistoryTable("TrangloRoles");
            });

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
               .HasMaxLength(300)
               .IsRequired()
               .HasColumnName("TrangloRoleCode");      

            builder.Property(o => o.Description)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("Description");

            builder.HasOne(o => o.TrangloDepartment)
             .WithMany()
             .HasForeignKey("TrangloDepartmentCode")
             .IsRequired(true);

            builder.HasOne(o => o.RoleStatus)
             .WithMany()
             .HasForeignKey("RoleStatusCode");

            builder.HasOne(o => o.AuthorityLevel)
             .WithMany()
             .HasForeignKey("AuthorityLevelCode");

            builder.Property(o => o.CreatorRole)
              .HasMaxLength(150)
             .HasColumnName("CreatorRoleCode");

            builder.Property(o => o.IsSuperApprover)
              .HasColumnName("IsSuperApprover");

            builder.Ignore(o => o.DomainEvents);
        }
    }
}
