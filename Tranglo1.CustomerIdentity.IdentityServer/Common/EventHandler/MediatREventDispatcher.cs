using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Infrastructure.Event;

namespace Tranglo1.CustomerIdentity.IdentityServer.Common.EventHandlers
{
	public class MediatREventDispatcher : IEventDispatcher
	{
		public MediatREventDispatcher(IMediator mediator)
		{
			Mediator = mediator;
		}

		public IMediator Mediator { get; }

		public Task DispatchAsync(DomainEvent @event)
		{
			INotification wrapper = EventWrapper<DomainEvent>.Create(@event);
			return this.Mediator.Publish(wrapper);
		}
	}
}
