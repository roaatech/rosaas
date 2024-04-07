using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Processors.Abstraction;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Factories
{
    public interface ISubscriptionRenewalFactory
    {
        SubscriptionRenewalProcessor InstantiateProcessor(SubscriptionRenewalTypeEnum renewalType);
    }
}