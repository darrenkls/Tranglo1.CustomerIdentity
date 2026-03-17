using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.ExternalUserRoleAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.ExternalUserRoleEntityTypeConfiguration
{
    class ExternalUserRoleEntityTypeConfiguration : BaseEntityTypeConfiguration<ExternalUserRole>
    {
        protected override void Configure(EntityTypeBuilder<ExternalUserRole> builder)
        {
            builder.ToTable("ExternalUserRoles", ExternalUserRoleDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(ExternalUserRoleDbContext.HISTORY_SCHEMA);
                config.HistoryTable("ExternalUserRoles");
            });

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
               .IsRequired()
               .HasColumnName("ExternalUserRoleCode");

            builder.Property(o => o.RoleCode)
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnName("RoleCode");
                //.HasComputedColumnSql("RC" + "[ExternalUserRoleCode]");

            builder.Property(o => o.ExternalUserRoleName)
                .HasMaxLength(100)
                .IsRequired()
                .HasColumnName("ExternalUserRoleName");

            builder.HasOne(o => o.ExternalUserRoleStatus)
             .WithMany()
             .IsRequired()
             .HasForeignKey("ExternalUserRoleStatusCode");

            builder.HasOne(o => o.Solution)
                .WithMany()
                .HasForeignKey("SolutionCode")
                .IsRequired(false);

            builder.Ignore(o => o.DomainEvents);
        }
    }
}
