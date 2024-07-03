using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Payment;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models
{
    public class SubscriptionRenewalDto
    {
        public Guid Id { get; set; }
        public AutoRenewalPlanDto Plan { get; set; } = new();
        public CustomLookupItemDto<Guid> Subscription { get; set; } = new();
        public CustomLookupItemDto<Guid> Product { get; set; } = new();
        public CustomLookupItemDto<Guid> ProductOwner { get; set; } = new();
        public string? Comment { get; set; }
        public DateTime EnabledDate { get; set; }
        public DateTime? SubscriptionRenewalDate { get; set; }
        public PaymentMethodCardDto? PaymentMethodCard { get; set; }
        public SubscriptionRenewalStatus Status { get; set; }
        public SubscriptionRenewalTypeEnum Type { get; set; }
        public int RenewalsCount { get; set; }
        public bool IsContinuousRenewal { get; set; }
        public class AutoRenewalPlanDto
        {
            public Guid Id { get; set; }
            public Guid PlanPriceId { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public PlanCycle Cycle { get; set; }
            public decimal Price { get; set; }
        }




    }


}
