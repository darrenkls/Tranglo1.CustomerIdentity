using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace Tranglo1.CustomerIdentity.Domain.Common
{
    /// <summary>
    /// This will determine the domain class whether is an aggregate root.
    /// All domain event will be added for aggregate root only.
    /// All these event will be handle when dispatch is called.
    /// 
    /// By add in new domain event from entity
    ///     base.AddDomainEvent(_domaineventname);
    /// </summary>
    public abstract class AggregateRoot : AggregateRoot<int>
    {
    }

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        private readonly List<DomainEvent> _domainEvents;
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

		protected AggregateRoot()
		{
            this._domainEvents = new List<DomainEvent>();
		}

        public void AddDomainEvent(DomainEvent eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(DomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

    }
}
