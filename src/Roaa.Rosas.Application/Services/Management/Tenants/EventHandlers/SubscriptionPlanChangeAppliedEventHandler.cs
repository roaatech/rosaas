using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionPlanChangeAppliedEventHandler : IInternalDomainEventHandler<SubscriptionPlanChangeAppliedEvent>
    {
        private readonly ILogger<SubscriptionPlanChangeAppliedEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;

        public SubscriptionPlanChangeAppliedEventHandler(IRosasDbContext dbContext,
                                                         IPublisher publisher,
                                                         ILogger<SubscriptionPlanChangeAppliedEventHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task Handle(SubscriptionPlanChangeAppliedEvent @event, CancellationToken cancellationToken)
        {

            _dbContext.SubscriptionPlanChangingHistories.Add(
             new SubscriptionPlanChangeHistory
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
             });

            await _dbContext.SaveChangesAsync(cancellationToken);

            var previousSubscriptionCycle = await _dbContext.SubscriptionCycles
                                                            .Where(x => x.Id == @event.PreviousSubscriptionCycleId)
                                                            .SingleOrDefaultAsync(cancellationToken);

            if (@event.SubscriptionPlanChange.Type == PlanChangingType.Upgrade)
            {
                await _publisher.Publish(new SubscriptionPlanUpgradedEvent(
                                         @event.Subscription,
                                         @event.SubscriptionPlanChange,
                                         previousSubscriptionCycle));
            }

            if (@event.SubscriptionPlanChange.Type == PlanChangingType.Downgrade)
            {
                await _publisher.Publish(new SubscriptionPlanDowngradedEvent(
                                         @event.Subscription,
                                         @event.SubscriptionPlanChange,
                                         previousSubscriptionCycle));
            }
        }
    }
}
