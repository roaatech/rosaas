using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.AutoRenewals.Models
{
    public class SubscriptionAutoRenewalDto
    {
        public Guid Id { get; set; }
        public AutoRenewalPlanDto Plan { get; set; } = new();
        public CustomLookupItemDto<Guid> Subscription { get; set; } = new();
        public CustomLookupItemDto<Guid> Product { get; set; } = new();
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }

        public class AutoRenewalPlanDto
        {
            public Guid Id { get; set; }
            public Guid PlanPriceId { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public PlanCycle Cycle { get; set; }
            public decimal Price { get; set; }
        }

        public class SubscriptionDto
        {
            public Guid Id { get; set; }
            public string SystemName { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }
    }


}
