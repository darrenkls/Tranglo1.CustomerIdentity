using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Identity;

namespace Tranglo1.CustomerIdentity.IdentityServer.DependencyInjection
{
	internal static class RoleCodeContextExtensions
	{
		/// <summary>
		/// Register implementation of <seealso cref="IPartnerContext"/> to retrieve
		/// current business profile Id submitted from API call.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddRoleCodeContext(this IServiceCollection services)
		{
			services.AddHttpContextAccessor();
			services.AddScoped<IRoleCodeContext, DefaultRoleCodeContext>();
			return services;
		}
	}
}
