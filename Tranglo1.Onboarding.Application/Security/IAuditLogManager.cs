using System.Threading;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Security
{
	public interface IAuditLogManager
	{
		Task LogAsync(AuditLog auditLog, CancellationToken cancellationToken);
	}
}
