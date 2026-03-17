using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
	internal static class BusinessProfileContextExtensions
	{
		/// <summary>
		/// Register implementation of <seealso cref="IBusinessProfileContext"/> to retrieve
		/// current business profile Id submitted from API call.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddBusinessProfileContext(this IServiceCollection services)
		{
			services.AddHttpContextAccessor();
			services.AddScoped<IBusinessProfileContext, DefaultBusinessProfileContext>();
			return services;
		}
	}
}
