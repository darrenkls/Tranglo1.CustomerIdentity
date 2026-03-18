using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Infrastructure.Event
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(DomainEvent @event);

    }
}
