using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Models
{
	public class VerifyCustomerUserEmailModel
	{
		public string UserId { get; set; }
		public string Token { get; set; }
	}
}
