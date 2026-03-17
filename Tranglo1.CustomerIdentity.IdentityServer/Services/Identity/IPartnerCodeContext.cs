using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Identity
{
	public interface IPartnerContext
	{
		Maybe<long> CurrentProfileId { get; }
	}

	internal class DefaultPartnerContext : IPartnerContext
	{
		public const string PartnerCode = "PartnerCode";

		public DefaultPartnerContext(IHttpContextAccessor httpContextAccessor)
		{
			if (httpContextAccessor == null)
			{
				throw new ArgumentException(nameof(httpContextAccessor));
			}

			HttpContextAccessor = httpContextAccessor;
		}

		public Maybe<long> CurrentProfileId
		{
			get
			{
				var _Id = HttpContextAccessor.HttpContext?.Items[PartnerCode];

				if (_Id == null)
				{
					return Maybe<long>.None;
				}

				return Maybe<long>.From((long)_Id);

			}
		}

		public IHttpContextAccessor HttpContextAccessor { get; }
	}
}
