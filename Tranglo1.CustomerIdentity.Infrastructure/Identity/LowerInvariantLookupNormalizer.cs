using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Infrastructure.Identity
{
	public sealed class LowerInvariantLookupNormalizer : ILookupNormalizer
	{
		public string NormalizeEmail(string email)
		{
			return email?.ToLowerInvariant();
		}

		public string NormalizeName(string name)
		{
			return name?.ToLowerInvariant();
		}
	}
}
