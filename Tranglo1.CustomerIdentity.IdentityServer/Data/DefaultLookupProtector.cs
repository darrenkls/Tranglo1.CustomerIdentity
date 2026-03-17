using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Data
{
	public class DefaultLookupProtector : ILookupProtector
	{
		private IDataProtectionProvider _Provider;
		private IDataProtector _protector;

		public DefaultLookupProtector(IDataProtectionProvider provider)
		{
			_Provider = provider;
			_protector = _Provider.CreateProtector("DefaultLookupProtector");
		}

		public string Protect(string keyId, string data)
		{
			return Convert.ToBase64String(_protector.Protect(Encoding.UTF8.GetBytes(data)));
		}

		public string Unprotect(string keyId, string data)
		{
			var _RawBytes = Convert.FromBase64String(data);
			var _p = _protector as IPersistedDataProtector;

			if (_p != null)
			{
				return Encoding.UTF8.GetString(
				_p.DangerousUnprotect(_RawBytes, true, out bool needMigration, out bool revoked));
			}

			return Encoding.UTF8.GetString(
				_protector.Unprotect(_RawBytes));
		}
	}

	public class DefaultLookupProtectorKeyRing : ILookupProtectorKeyRing
	{
		public string this[string keyId] => keyId;

		public string CurrentKeyId => "Key1";

		public IEnumerable<string> GetAllKeyIds()
		{
			yield return "Key1";
		}
	}
}
