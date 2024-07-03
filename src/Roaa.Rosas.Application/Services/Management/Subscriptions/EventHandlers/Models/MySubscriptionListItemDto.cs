using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Payment;

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
        public PlanPriceDto PlanPrice { get; set; } = new();
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public CustomLookupItemDto<Guid> Product { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool AutoRenewalIsEnabled { get; set; }
        public bool UpgradingIsEnabled { get; set; }
        public bool DowngradingIsEnabled { get; set; }

        [Obsolete("This property is obsolete. Use UpgradingIsEnabled or DowngradingIsEnabled instead.", false)]
        public bool PlanChangingIsEnabled { get; set; }

        [Obsolete("This property is obsolete. Use SubscriptionRenewalAction instead.", false)]
        public bool IsPlanChangeAllowed { get; set; }

        [Obsolete("This property is obsolete.", false)]
        public SubscriptionRenewalTypeEnum? PlanChangingType { get; set; }
        public SubscriptionMode SubscriptionMode { get; set; }
        public PaymentMethodCardDto? PaymentMethodCard { get; set; }
        public TrialSubscriptionDto? Trial { get; set; }
        public SubscriptionRenewalAction? SubscriptionRenewalAction { get; set; }
        public CustomLookupItemDto<Guid>? ProductOwner { get; set; }

        public class TrialSubscriptionDto
        {
            public Guid TrialPlanId { get; set; }
            public Guid TrialPlanPriceId { get; set; }
            public Guid SelectedPlanId { get; set; }
            public Guid SelectedPlanPriceId { get; set; }
            public DateTime EndDate { get; set; }
            public int TrialPeriodInDays { get; set; }
        }

    }

    public class SubscriptionRenewalAction
    {
        public bool EnableAutoRenual { get; set; }
        public bool CancelAutoRenual { get; set; }
        public bool EnabelUpgrading { get; set; }
        public bool CancelUpgrading { get; set; }
        public bool EnabelDowngrading { get; set; }
        public bool CancelDowngrading { get; set; }
    }
}
