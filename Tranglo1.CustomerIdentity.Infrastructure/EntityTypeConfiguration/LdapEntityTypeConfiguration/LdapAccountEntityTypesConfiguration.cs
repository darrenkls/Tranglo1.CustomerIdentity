using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.ActiveDirectory;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.LdapEntityTypeConfiguration
{
	class LdapAccountEntityTypesConfiguration : BaseEntityTypeConfiguration<LdapAccount>
	{
		protected override void Configure(EntityTypeBuilder<LdapAccount> builder)
		{
			builder.ToTable("LdapAccounts", "dbo").HasTemporalTable();
			builder.Property(a => a.Name).HasMaxLength(255).IsRequired(true).IsUnicode(true);
			builder.HasKey(a => a.SamAccountName);
			builder.Property(a => a.Name).IsRequired(true).HasMaxLength(300).IsUnicode(true);
			builder.Property(a => a.IsEnabled).IsRequired(true);
			builder.Property(a => a.EmailAddress).HasMaxLength(320).IsRequired(false).IsUnicode(true);
		}

		protected override bool HasCreationInfo()
		{
			return false;
		}

		protected override bool HasLastModificationInfo()
		{
			return false;
		}
	}
}
