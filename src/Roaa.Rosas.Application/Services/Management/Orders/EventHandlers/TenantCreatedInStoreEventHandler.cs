using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.EventHandlers
{
    public class TenantCreatedInStoreEventHandler : IInternalDomainEventHandler<TenantCreatedInStoreEvent>
    {
        private readonly ILogger<TenantCreatedInStoreEventHandler> _logger;
        private readonly IOrderService _orderService;
        private readonly IIdentityContextService _identityContextService;

        public TenantCreatedInStoreEventHandler(IIdentityContextService identityContextService,
                                                IOrderService orderService,
                                                ILogger<TenantCreatedInStoreEventHandler> logger)
        {
            _identityContextService = identityContextService;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task Handle(TenantCreatedInStoreEvent @event, CancellationToken cancellationToken)
        {
            List<Subscription> subscriptions = @event.Tenant.Subscriptions?.ToList() ?? new List<Subscription>();
            await _orderService.SetSubscriptionIdToOrderItemsAsync(@event.Tenant.LastOrderId, @event.Tenant.Id, subscriptions, cancellationToken);
        }


    }
}
