using Microsoft.AspNetCore.Identity;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Entities.Identity
{
    public partial class User : IdentityUser<Guid>, IBaseEntity
    {

        public User()
        {
            Id = Guid.NewGuid();
            SecurityStamp = Guid.NewGuid().ToString();
        }



        public UserStatus Status { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public string? Locale { get; set; }
        public string? MetaData { get; set; }
        public bool IsDeleted { get; set; }



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
}