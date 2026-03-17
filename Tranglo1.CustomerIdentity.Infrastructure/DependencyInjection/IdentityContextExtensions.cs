using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IdentityContextExtensions
	{
		public static IServiceCollection AddBackendIdentityContext(this IServiceCollection services)
		{
			services.AddSingleton<IIdentityContext, BackendIdentityContext>();
			return services;
		}
	}
}
