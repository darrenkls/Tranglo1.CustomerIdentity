using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class ApplicationUserClaimEntityTypeConfiguration : BaseEntityTypeConfiguration<ApplicationUserClaim>
	{
		protected override void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
		{
			builder.ToTable("ApplicationUserClaims");
			builder.HasKey(claim => claim.Id);
			builder.Property(claim => claim.ClaimType)
				.HasMaxLength(30)
				.IsRequired(true)
				.IsUnicode(false);

		}
	}
}
