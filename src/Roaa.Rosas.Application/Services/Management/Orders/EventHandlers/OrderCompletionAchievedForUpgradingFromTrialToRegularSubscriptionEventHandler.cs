using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Subscriptions.AutoRenewals;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.EventHandlers
{
    public class OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEventHandler : IInternalDomainEventHandler<OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent>
    {
        private readonly ILogger<OrderCompletionAchievedForTenantCreationEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionAutoRenewalService _subscriptionAutoRenewalService;
        private readonly ITenantService _tenantService;
        private readonly ISender _mediator;

        public OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEventHandler(ITenantWorkflow workflow,
                                            IRosasDbContext dbContext,
                                        IIdentityContextService identityContextService,
                                        ISubscriptionService subscriptionService,
                                        ITenantService tenantService,
                                        ISubscriptionAutoRenewalService subscriptionAutoRenewalService,
                                        ISender mediator,
                                        ILogger<OrderCompletionAchievedForTenantCreationEventHandler> logger)
        {
            _workflow = workflow;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _subscriptionService = subscriptionService;
            _tenantService = tenantService;
            _subscriptionAutoRenewalService = subscriptionAutoRenewalService;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent @event, CancellationToken cancellationToken)
        {
            var tenantCreationRequest = await _dbContext.TenantCreationRequests
                                                        .Include(x => x.Specifications)
                                                        .Where(x => x.OrderId == @event.OrderId)
                                                        .SingleOrDefaultAsync(cancellationToken);
            if (tenantCreationRequest is null)
            {
                throw new NullReferenceException($"The tenantCreationRequest of order [OrderId:{@event.OrderId}] can't be null.");

            }
            var tenantId = tenantCreationRequest.Id;

            var subscriptions = await _dbContext.Subscriptions
                                                    .Include(x => x.Plan)
                                                    .Include(x => x.PlanPrice)
                                                    .Where(x => x.TenantId == tenantId)
                                                    .ToListAsync(cancellationToken);


            foreach (var subscription in subscriptions)
            {
                if (subscription.SubscriptionMode == SubscriptionMode.PendingToNormal || subscription.SubscriptionMode == SubscriptionMode.Trial)
                {
                    var orderItem = await _dbContext.OrderItems
                                         .Where(x => x.OrderId == @event.OrderId && x.SubscriptionId == subscription.Id)
                                         .Select(x => new { x.SubscriptionId, x.PlanId, x.PlanPriceId })
                                         .SingleOrDefaultAsync(cancellationToken);

                    if (orderItem is null)
                    {
                        throw new NullReferenceException($"The orderItem can't be null.");
                    }

                    await _subscriptionService.ResetSubscriptionPlanAsync(subscription, orderItem.PlanId, orderItem.PlanPriceId, true, SubscriptionMode.Normal);

                    //if (tenantCreationRequest.AutoRenewalIsEnabled)
                    //{
                    //    await _subscriptionAutoRenewalService.EnableAutoRenewalAsync(subscription.Id, @event.CardReferenceId, @event.PaymentPlatform, subscription.PlanPriceId, null, cancellationToken);
                    //}
                }
            }
        }

    }
}
