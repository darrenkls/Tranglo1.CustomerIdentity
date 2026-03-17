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
    public class BusinessProfileTemporalTimeDbContextFactory : IDesignTimeDbContextFactory<BusinessProfileDbContext>
    {
        public BusinessProfileDbContext CreateDbContext(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();

            DbContextOptionsBuilder<BusinessProfileDbContext> optionsBuilder =
                new DbContextOptionsBuilder<BusinessProfileDbContext>();

            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            IServiceCollection services = new ServiceCollection();

            services.AddDbContext<BusinessProfileDbContext>((provider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseInternalServiceProvider(provider);
            });

            services.AddEntityFrameworkSqlServer();
            services.RegisterTemporalTablesForDatabase();

            var provider = services.BuildServiceProvider();

            return provider.GetService<BusinessProfileDbContext>();
        }

    }
}
