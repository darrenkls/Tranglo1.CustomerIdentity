using EntityFrameworkCore.SqlServer.TemporalTable.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.IdentityServer.FactoryDesignTime
{
    public class ScreeningTimeDbContextFactory : IDesignTimeDbContextFactory<ScreeningDBContext>
    {
        public ScreeningDBContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ScreeningDBContext> optionsBuilder =
                new DbContextOptionsBuilder<ScreeningDBContext>();

            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            IServiceCollection services = new ServiceCollection();

            services.AddDbContext<ScreeningDBContext>((provider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseInternalServiceProvider(provider);
            });

            services.AddEntityFrameworkSqlServer();
            services.RegisterTemporalTablesForDatabase();

            var provider = services.BuildServiceProvider();

            return provider.GetService<ScreeningDBContext>();
        }
    }
}
