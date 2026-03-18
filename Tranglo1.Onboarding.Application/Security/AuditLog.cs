using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.IdentityServer.Security
{
	public class AuditLog
	{
		public string Username { get; set; }
		public UserType UserType { get; set; }
		public DateTime EventDate { get; set; }
		public string ActionDescription { get; set; }
		public string ModuleName { get; set; }
		public IPAddress ClientAddress { get; set; }
		public string CorrelationId { get; set; }
	}
}
