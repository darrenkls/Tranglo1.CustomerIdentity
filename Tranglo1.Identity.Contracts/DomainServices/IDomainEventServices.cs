using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.DomainServices
{
    public interface IDomainEventServices
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        void AddDomainEvent(IDomainEvent eventItem);
        void ClearDomainEvents();
        void RemoveDomainEvent(IDomainEvent eventItem);
    }
}
