using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class UserVerificationTokenEntityTypeConfiguration : BaseEntityTypeConfiguration<UserVerificationToken>
    {
        protected override void Configure(EntityTypeBuilder<UserVerificationToken> builder)
        {
            builder.ToTable("UserVerificationTokens", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.Property(a => a.Token)
             .HasMaxLength(320)
             .IsRequired(false);
        }
    }
}