using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionTrials
{
    public interface ITrialProcessingService
    {
        bool HasTrial(SubscriptionPreparationModel model);

        int? FeatchTrialPeriodInDays(SubscriptionPreparationModel model);

        TrialSubscription? BuildSubscriptionTrialPeriodEntity(SubscriptionPreparationModel model);
    }
}
