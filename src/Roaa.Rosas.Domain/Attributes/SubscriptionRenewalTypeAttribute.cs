using Roaa.Rosas.Domain.Attributes;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubscriptionRenewalTypeAttribute : BaseCustomAttribute
    {
        public SubscriptionRenewalTypeEnum RenewalType { get; set; }

        public SubscriptionRenewalTypeAttribute(SubscriptionRenewalTypeEnum renewalType) : base(renewalType.ToString())
        {
            RenewalType = renewalType;
        }
    }

}
