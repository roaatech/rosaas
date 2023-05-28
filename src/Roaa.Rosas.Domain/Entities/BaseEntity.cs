using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; }



        #region Domain Events

        private readonly List<BaseInternalEvent> _domainEvents = new();

        public IReadOnlyCollection<BaseInternalEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(BaseInternalEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseInternalEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        #endregion
    }
    public interface IBaseEntity
    {
        public Guid Id { get; set; }



        #region Domain Events 
        public IReadOnlyCollection<BaseInternalEvent> DomainEvents { get; }

        public void AddDomainEvent(BaseInternalEvent domainEvent);

        public void RemoveDomainEvent(BaseInternalEvent domainEvent);

        public void ClearDomainEvents();

        #endregion
    }
}
