using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models
{
    public record SubscriptionRenewalPreparationModel
    {
        public Subscription Subscription { get; set; } = new();
        public SubscriptionRenewal SubscriptionRenewal { get; set; } = new();

        public SubscriptionRenewalPreparationModel()
        {
        }

        public SubscriptionRenewalPreparationModel(Subscription subscription,
                                                    SubscriptionRenewal subscriptionRenewal)
        {
            Subscription = subscription;
            SubscriptionRenewal = subscriptionRenewal;
        }
    }


    public record SubscriptionRenewalPreparationResult
    {
        public SubscriptionRenewalPreparationResult(bool applySubscriptionRenewalByExternalSystemAction)
        {
            ApplySubscriptionRenewalByExternalSystemAction = applySubscriptionRenewalByExternalSystemAction;
        }

        public bool ApplySubscriptionRenewalByExternalSystemAction { get; set; } = new();
    }
}
