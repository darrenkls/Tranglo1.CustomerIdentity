using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.DomainServices;

namespace Tranglo1.CustomerIdentity.Domain.Common
{
    public abstract class DomainEventService : DomainEventService<int>
    {
    }

    public abstract class DomainEventService<TKey> : Entity<TKey>
    {
        private readonly List<IDomainEvent> _domainEvents;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        protected DomainEventService()
        {
            this._domainEvents = new List<IDomainEvent>();
        }

        public void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

    }
}
