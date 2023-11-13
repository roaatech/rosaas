using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionPlanUpgradedEventHandler : IInternalDomainEventHandler<SubscriptionPlanUpgradedEvent>
    {
        private readonly ILogger<SubscriptionPlanUpgradedEventHandler> _logger;
        private readonly IPublisher _publisher;

        public SubscriptionPlanUpgradedEventHandler(IPublisher publisher,
                                                      ILogger<SubscriptionPlanUpgradedEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(SubscriptionPlanUpgradedEvent @event, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SubscriptionUpgraded,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: string.Empty,
                                                   processId: out _,
                                                   subscriptions: @event.Subscription));
        }
    }
}
