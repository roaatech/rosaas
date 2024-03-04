using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.Models
{
    public record MySubscriptionListItemDto
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool AutoRenewalIsEnabled { get; set; }
        public bool PlanChangingIsEnabled { get; set; }
        public PlanChangingType? PlanChangingType { get; set; }
        public PaymentMethodCardDto? PaymentMethodCard { get; set; }
    }
}
