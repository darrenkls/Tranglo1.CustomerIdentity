using System.Collections.Generic;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.ActiveDirectory;

namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
	public interface ILdapAccountRepository
	{
		Task SaveAsync(LdapAccount account);
		Task<IReadOnlyCollection<LdapAccount>> GetLdapAccountsAsync();
	}
}