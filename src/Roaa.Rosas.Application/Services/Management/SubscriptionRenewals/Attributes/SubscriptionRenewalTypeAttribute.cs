using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubscriptionRenewalTypeAttribute : Attribute
    {
        public SubscriptionRenewalTypeEnum RenewalType { get; set; }

        public SubscriptionRenewalTypeAttribute(SubscriptionRenewalTypeEnum renewalType)
        {
            RenewalType = renewalType;
        }
    }

}
