using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class TrialSubscriptionUpgradedToStandardEventHandler : BaseWebhookEventHandler<TrialSubscriptionUpgradedToStandardEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.TrialSubscriptionUpgradedToStandard;



        public TrialSubscriptionUpgradedToStandardEventHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }



        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var tenantSystemName = await DbContext!.Subscriptions.Where(x => x.Id == Event!.Trial.SubscriptionId)
                                                          .Select(x => x.Tenant!.SystemName)
                                                          .SingleOrDefaultAsync(cancellationToken);

            var plans = await DbContext!.Plans.Where(x => x.Id == Event!.Trial.TrialPlanId ||
                                                                     x.Id == Event!.Trial.SelectedPlanId)
                                                    .Select(x => new { x.Id, x.SystemName })
                                                    .ToListAsync(cancellationToken);

            ArgumentNullException.ThrowIfNull(tenantSystemName);

            var metadata = new
            {
                TrialPlanSystemName = plans.Where(x => x.Id == Event!.Trial.TrialPlanId).SingleOrDefault(),
                StandardPlanSystemName = plans.Where(x => x.Id == Event!.Trial.SelectedPlanId).SingleOrDefault(),
                TrialPeriodInDays = Event!.Trial.TrialPeriodInDays,
                Date = DateTime.UtcNow,
            };

            return (metadata, Event!.ProductId, tenantSystemName);
        }
    }
}
