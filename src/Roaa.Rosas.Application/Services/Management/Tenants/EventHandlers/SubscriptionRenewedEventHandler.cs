using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionRenewedEventHandler : IInternalDomainEventHandler<SubscriptionRenewedEvent>
    {
        private readonly ILogger<SubscriptionRenewedEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;

        public SubscriptionRenewedEventHandler(IRosasDbContext dbContext,
                                               IPublisher publisher,
                                               ILogger<SubscriptionRenewedEventHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task Handle(SubscriptionRenewedEvent @event, CancellationToken cancellationToken)
        {

            _dbContext.SubscriptionAutoRenewalHistories.Add(
                new SubscriptionAutoRenewalHistory
                {
                    Id = Guid.NewGuid(),
                    SubscriptionId = @event.SubscriptionAutoRenewal.SubscriptionId,
                    PlanId = @event.SubscriptionAutoRenewal.PlanId,
                    PlanPriceId = @event.SubscriptionAutoRenewal.PlanPriceId,
                    Price = @event.SubscriptionAutoRenewal.Price,
                    AutoRenewalEnabledByUserId = @event.SubscriptionAutoRenewal.ModifiedByUserId,
                    AutoRenewalEnabledDate = @event.SubscriptionAutoRenewal.ModificationDate,
                    Comment = @event.SubscriptionAutoRenewal.Comment,
                    RenewalDate = DateTime.UtcNow,
                    PlanCycle = @event.SubscriptionAutoRenewal.PlanCycle,

                });

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SubscriptionRenewed,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: @event.SystemComment,
                                                   processId: out _,
                                                   subscriptions: @event.Subscription));
        }
    }
}
