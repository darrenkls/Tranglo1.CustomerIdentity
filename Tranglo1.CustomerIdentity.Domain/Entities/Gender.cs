using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
	public class Gender : Enumeration
	{
		public Gender() : base()
		{

		}
		public Gender(int id, string name)
			: base(id, name)
		{

		}

		public static readonly Gender Male = new Gender(1, "Male");
		public static readonly Gender Female = new Gender(2, "Female");
	}
}
