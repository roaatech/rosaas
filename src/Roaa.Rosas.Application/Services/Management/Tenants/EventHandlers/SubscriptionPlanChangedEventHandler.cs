using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionPlanChangedEventHandler : IInternalDomainEventHandler<SubscriptionPlanChangedEvent>
    {
        private readonly ILogger<SubscriptionPlanChangedEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;

        public SubscriptionPlanChangedEventHandler(IRosasDbContext dbContext,
                                                   ILogger<SubscriptionPlanChangedEventHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Handle(SubscriptionPlanChangedEvent @event, CancellationToken cancellationToken)
        {
            var previousSubscriptionCycle = await _dbContext.SubscriptionCycles
                                                            .Where(x => x.Id == @event.PreviousSubscriptionCycleId)
                                                            .SingleOrDefaultAsync(cancellationToken);

            var subscriptionPlanChangeHistory = new SubscriptionPlanChangeHistory
            {
                Id = Guid.NewGuid(),
                SubscriptionId = @event.SubscriptionPlanChange.SubscriptionId,
                PlanId = @event.SubscriptionPlanChange.PlanId,
                PlanPriceId = @event.SubscriptionPlanChange.PlanPriceId,
                Type = @event.SubscriptionPlanChange.Type,
                PlanCycle = @event.SubscriptionPlanChange.PlanCycle,
                Price = @event.SubscriptionPlanChange.Price,
                PlanChangeEnabledByUserId = @event.SubscriptionPlanChange.ModifiedByUserId,
                PlanChangeEnabledDate = @event.SubscriptionPlanChange.ModificationDate,
                Comment = @event.SubscriptionPlanChange.Comment,
                ChangeDate = DateTime.UtcNow,
            };

            if (@event.SubscriptionPlanChange.Type == PlanChangingType.Upgrade)
            {
                subscriptionPlanChangeHistory.AddDomainEvent(new SubscriptionPlanUpgradedEvent(
                                         @event.Subscription,
                                         @event.SubscriptionPlanChange,
                                         previousSubscriptionCycle));
            }

            if (@event.SubscriptionPlanChange.Type == PlanChangingType.Downgrade)
            {
                subscriptionPlanChangeHistory.AddDomainEvent(new SubscriptionPlanDowngradedEvent(
                                         @event.Subscription,
                                         @event.SubscriptionPlanChange,
                                         previousSubscriptionCycle));
            }

            _dbContext.SubscriptionPlanChangingHistories.Add(subscriptionPlanChangeHistory);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
