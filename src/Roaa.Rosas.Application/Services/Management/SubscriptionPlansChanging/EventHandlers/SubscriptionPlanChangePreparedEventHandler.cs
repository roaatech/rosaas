using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionPlansChanging.EventHandlers
{
    public class SubscriptionPlanChangePreparedEventHandler : IInternalDomainEventHandler<SubscriptionPlanChangePreparedEvent>
    {
        private readonly ILogger<SubscriptionPlanChangePreparedEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;

        public SubscriptionPlanChangePreparedEventHandler(IRosasDbContext dbContext,
                IPublisher publisher,
                                                   ILogger<SubscriptionPlanChangePreparedEventHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task Handle(SubscriptionPlanChangePreparedEvent @event, CancellationToken cancellationToken)
        {

            if (@event.SubscriptionPlanChange.Type == PlanChangingType.Upgrade)
            {
                await _publisher.Publish(new SubscriptionPlanUpgradePreparedEvent(
                                         @event.Subscription,
                                         @event.SubscriptionPlanChange,
                                         null));
            }

            if (@event.SubscriptionPlanChange.Type == PlanChangingType.Downgrade)
            {
                await _publisher.Publish(new SubscriptionPlanDowngradePreparedEvent(
                                         @event.Subscription,
                                         @event.SubscriptionPlanChange,
                                         null));
            }

        }
    }
}
