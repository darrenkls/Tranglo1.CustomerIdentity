using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
	public class ActionOperation : Enumeration
	{
		public ActionOperation() : base()
		{

		}
		public ActionOperation(int id, string name)
			: base(id, name)
		{

		}

		public static readonly ActionOperation Create = new ActionOperation(1, "Create");
		public static readonly ActionOperation Delete = new ActionOperation(2, "Delete");
	}
}
