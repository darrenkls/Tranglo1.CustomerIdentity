using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tranglo1.CustomerIdentity.IdentityServer.Infrastructure.Persistance
{
	internal class ProtectionKeyDbContext : DbContext, IDataProtectionKeyContext
    {
        public ProtectionKeyDbContext(DbContextOptions<ProtectionKeyDbContext> options)
            : base(options) { }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
