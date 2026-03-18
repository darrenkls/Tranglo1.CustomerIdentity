using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Common
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }

        void AddDomainEvent(DomainEvent eventItem);
        void ClearDomainEvents();
        void RemoveDomainEvent(DomainEvent eventItem);
    }
}
