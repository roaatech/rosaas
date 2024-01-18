using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
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
        private readonly ITenantService _tenantService;
        private readonly ISender _mediator;

        public OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEventHandler(ITenantWorkflow workflow,
                                            IRosasDbContext dbContext,
                                            IIdentityContextService identityContextService,
                                            ISubscriptionService subscriptionService,
                                            ITenantService tenantService,
                                            ISender mediator,
                                            ILogger<OrderCompletionAchievedForTenantCreationEventHandler> logger)
        {
            _workflow = workflow;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _subscriptionService = subscriptionService;
            _tenantService = tenantService;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent @event, CancellationToken cancellationToken)
        {
            var tenantCreationRequest = await _dbContext.TenantCreationRequests
                                                        .Include(x => x.Specifications)
                                                        .Where(x => x.OrderId == @event.OrderId)
                                                        .SingleOrDefaultAsync(cancellationToken);

            var tenantId = await _dbContext.Tenants
                                .Where(x => tenantCreationRequest.NormalizedSystemName
                                                                 .ToLower()
                                                                 .Equals(x.SystemName))
                                .Select(x => x.Id)
                                .SingleOrDefaultAsync(cancellationToken);

            if (tenantId == Guid.Empty)
            {
                throw new NullReferenceException($"The tenantId can't be null.");

            }
            var subscriptions = await _dbContext.Subscriptions
                                                    .Include(x => x.Plan)
                                                    .Include(x => x.PlanPrice)
                                                    .Where(x => x.TenantId == tenantId)
                                                    .ToListAsync(cancellationToken);

            var date = DateTime.UtcNow;

            foreach (var subscription in subscriptions)
            {
                if (subscription.SubscriptionMode == SubscriptionMode.PendingToRegular || subscription.SubscriptionMode == SubscriptionMode.Trial)
                {
                    await _subscriptionService.ResetSubscriptionPlanAsync(subscription, true, SubscriptionMode.Regular);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
