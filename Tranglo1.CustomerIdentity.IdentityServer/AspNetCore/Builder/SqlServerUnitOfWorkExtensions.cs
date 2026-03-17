using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
	public static class SqlServerUnitOfWorkExtensions
	{
		public static IApplicationBuilder UseSqlServerUnitOfWork(this IApplicationBuilder builder)
		{
			builder.Use(async (context, next) =>
			{
				await using (IUnitOfWork unitOfWork = context.RequestServices.GetService<IUnitOfWork>())
				{
					await next.Invoke();
					await unitOfWork.CommitAsync();
				}
			});

			return builder;
		}
	}
}
