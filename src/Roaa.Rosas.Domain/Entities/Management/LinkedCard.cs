using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class LinkedCard : BaseEntity
    {
        public string ReferenceId { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public PaymentPlatform PaymentPlatform { get; set; }
    }
}
