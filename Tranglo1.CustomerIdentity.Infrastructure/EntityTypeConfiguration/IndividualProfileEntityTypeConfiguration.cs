using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class IndividualProfileEntityTypeConfiguration : BaseEntityTypeConfiguration<IndividualProfile>
    {
        protected override void Configure(EntityTypeBuilder<IndividualProfile> builder)
        {
            builder.ToTable("IndividualProfiles", ApplicationUserDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("IndividualProfileCode");

            builder.HasOne(o => o.Solution)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey("SolutionCode");

            builder.HasOne(o => o.CustomerUser)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey("UserId");

        }
    }
}
