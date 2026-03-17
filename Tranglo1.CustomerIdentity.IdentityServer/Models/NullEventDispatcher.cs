using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Infrastructure.Event;

namespace Tranglo1.CustomerIdentity.IdentityServer.Models
{
	internal class NullEventDispatcher : IEventDispatcher
	{
		public Task DispatchAsync(DomainEvent @event)
		{
			return Task.CompletedTask;
		}
	}
}
