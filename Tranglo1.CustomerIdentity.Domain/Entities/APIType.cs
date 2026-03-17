using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
	public class APIType : Enumeration
	{
		public APIType() : base()
		{

		}

		public APIType(int id, string name)
			: base(id, name)
		{

		}

		public static readonly APIType URL = new APIType(1, "URL");
		public static readonly APIType WSDL = new APIType(2, "WSDL");
	}
}
