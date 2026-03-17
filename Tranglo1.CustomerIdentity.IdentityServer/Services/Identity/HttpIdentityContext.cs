using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Identity
{
	/// <summary>
	/// The lifetime of this implementation is expected to same as <seealso cref="IHttpContextAccessor"/>, which is 
	/// Scoped
	/// </summary>
	public class HttpIdentityContext : IIdentityContext
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public HttpIdentityContext(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		public System.Security.Claims.ClaimsPrincipal CurrentUser => httpContextAccessor.HttpContext.User;
	}
}
