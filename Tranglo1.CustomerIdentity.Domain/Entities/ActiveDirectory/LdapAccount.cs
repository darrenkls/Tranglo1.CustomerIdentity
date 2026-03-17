using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.ActiveDirectory
{
	[DebuggerDisplay("{SamAccountName,nq} ({Name,nq}, {EmailAddress,nq}) : {IsEnabled}")]
	public class LdapAccount
	{
		/// <summary>
		/// This is the Tranglo AD login id.
		/// </summary>
		public string SamAccountName { get; set; }

		/// <summary>
		/// Tranglo staff's fullname
		/// </summary>
		public string Name { get; set; }
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Tranglo staff email address
		/// </summary>
		public string EmailAddress { get; set; }
	}
}
