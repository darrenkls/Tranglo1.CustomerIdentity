using System;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Security;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services
{
	public class EntityFrameworkAuditLogManager : IAuditLogManager
	{
		public Task LogAsync(AuditLog auditLog, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
