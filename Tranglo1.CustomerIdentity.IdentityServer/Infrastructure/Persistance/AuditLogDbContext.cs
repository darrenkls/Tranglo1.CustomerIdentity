using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Security;

namespace Tranglo1.CustomerIdentity.IdentityServer.Infrastructure.Persistance
{
	class AuditLogDbContext : DbContext
	{
		public DbSet<AuditLog> AuditLogs { get; set; }

		public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options)
			: base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AuditLog>(entity=>
			{
				entity.ToTable("AuditLogs");

				entity.Property<long>("LogId")
					.IsRequired(true)
					.UseHiLo("AuditLogId");

				entity.Property(e => e.Username)
					.IsRequired(false);

				entity.Property(e => e.ActionDescription)
					.IsRequired(true)
					.IsUnicode(true);

				entity.Property(e => e.CorrelationId)
					.HasMaxLength(36);

				entity.Property(e => e.EventDate)
					.IsRequired(true)
					.HasConversion(d => d, d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

				entity
					.HasKey("LogId").IsClustered(true)
					.HasName("pk_AuditLogs");

				entity.Property(e => e.ModuleName)
					.IsRequired(false)
					.IsUnicode(true)
					.HasMaxLength(150);

				entity.Property(e => e.ClientAddress)
					.IsRequired(false)
					//https://superuser.com/questions/381022/how-many-characters-can-an-ip-address-be
					.HasColumnType("varchar(39)")
					.HasConversion(d => d.ToString(), d => IPAddress.Parse(d));
				
			});

			modelBuilder.HasSequence("AuditLogId")
				.HasMax(long.MaxValue)
				.HasMin(1)
				.IncrementsBy(20)
				.IsCyclic(false);
		}
	}
}
