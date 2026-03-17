using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes
{
   public class SignUpAccountStatus : Enumeration
	{
		public SignUpAccountStatus() : base()
		{

		}

		public SignUpAccountStatus(int id, string name)
			: base(id, name)
		{

		}


		public static readonly SignUpAccountStatus Active = new SignUpAccountStatus(1, "Active");
		public static readonly SignUpAccountStatus Used = new SignUpAccountStatus(2, "Used");
		public static readonly SignUpAccountStatus Expired = new SignUpAccountStatus(3, "Expired");


	}
}