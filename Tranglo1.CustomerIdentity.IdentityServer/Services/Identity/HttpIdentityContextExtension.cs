using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Identity;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class HttpIdentityContextExtension
	{
		/// <summary>
		/// Register instance of <seealso cref="IIdentityContext"/>. 
		/// Use this extension when application type is ASP.NET Core
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddHttpIdentityContext(this IServiceCollection services)
		{
			services.AddHttpContextAccessor();
			
			//The lifetime should be scope instead of transient or singleton.
			services.AddScoped<IIdentityContext, HttpIdentityContext>();
			return services;
		}
	}
}
