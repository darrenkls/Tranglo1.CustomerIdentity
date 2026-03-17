using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class ExternalUserRoleStatus : Enumeration
    {
		public ExternalUserRoleStatus() : base()
		{

		}

		public ExternalUserRoleStatus(int id, string name)
			: base(id, name)
		{

		}

		public static readonly ExternalUserRoleStatus Active = new ExternalUserRoleStatus(1, "Active");
		public static readonly ExternalUserRoleStatus Inactive = new ExternalUserRoleStatus(2, "Inactive");
	}
}
