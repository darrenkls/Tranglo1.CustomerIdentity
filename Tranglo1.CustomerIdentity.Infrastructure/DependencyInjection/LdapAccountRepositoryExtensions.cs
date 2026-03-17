using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Identity;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Tranglo1.CustomerIdentity.Infrastructure.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class LdapAccountRepositoryExtensions
	{
		public static IServiceCollection AddLdapRepository(this IServiceCollection services,
			Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = null)
		{
			if (services is null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddScoped<ILdapAccountRepository, LdapAccountRepository>();
			services.AddDbContext<LdapAccountDbContext>(optionsAction);
			services.AddNullEventDispatcher();
			services.AddBackendIdentityContext();

			return services;
		}

		public static IServiceCollection AddLdapRepository(this IServiceCollection services,
			Action<DbContextOptionsBuilder> optionsAction = null)
		{
			if (services is null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddScoped<ILdapAccountRepository, LdapAccountRepository>();
			services.AddDbContext<LdapAccountDbContext>(optionsAction);
			services.AddNullEventDispatcher();
			services.AddBackendIdentityContext();

			return services;
		}

		public static IServiceCollection AddLdapPasswordValidator(this IServiceCollection services, LdapConfiguration ldapConfiguration, 
			Action<LdapConnectionOptions> configureOptions )
		{
			if (ldapConfiguration == null)
			{
				throw new ArgumentException(nameof(ldapConfiguration));
			}

			services.AddSingleton<LdapConfiguration>(ldapConfiguration);

			LdapConnectionOptions connectionOptions = new LdapConnectionOptions();
			configureOptions?.Invoke(connectionOptions);
			services.AddSingleton<LdapConnectionOptions>(connectionOptions);

			services.AddScoped<IPasswordHasher<ApplicationUser>, LdapPasswordHasher>();

			return services;
		}
	}
}
