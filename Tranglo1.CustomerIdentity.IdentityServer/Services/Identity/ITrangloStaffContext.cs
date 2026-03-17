using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Identity
{
    public interface ITrangloStaffContext
    {
		internal class DefaultTrangloStaffContext : ITrangloStaffContext
		{
			public const string TrangloStaffLoginId = "TrangloStaffLoginId";

			public DefaultTrangloStaffContext(IHttpContextAccessor httpContextAccessor)
			{
				if (httpContextAccessor == null)
				{
					throw new ArgumentException(nameof(httpContextAccessor));
				}

				HttpContextAccessor = httpContextAccessor;
			}
			public IHttpContextAccessor HttpContextAccessor { get; }
		}
		}
	}
