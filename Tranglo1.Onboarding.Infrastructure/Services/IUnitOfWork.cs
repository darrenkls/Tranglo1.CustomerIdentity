using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Repositories;

namespace Tranglo1.CustomerIdentity.Infrastructure.Services
{
	public interface IUnitOfWork : IDisposable, IAsyncDisposable
	{
		DbConnection Connection { get; }
		DbTransaction Transaction { get; }
		Task CommitAsync();
		Task RollbackAsync();
	}
}
