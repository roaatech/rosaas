using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.TenantCreationRequests;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.EventHandlers
{
    public class PaymentProcessingPreparationEventHandler : IInternalDomainEventHandler<PaymentProcessingPreparationEvent>
    {
        private readonly ILogger<PaymentProcessingPreparationEventHandler> _logger;
        private readonly ITenantCreationRequestService _tenantCreationRequestService;



        public PaymentProcessingPreparationEventHandler(ITenantCreationRequestService tenantCreationRequestService,
                                                                    ILogger<PaymentProcessingPreparationEventHandler> logger)
        {
            _tenantCreationRequestService = tenantCreationRequestService;
            _logger = logger;
        }




        public async Task Handle(PaymentProcessingPreparationEvent @event, CancellationToken cancellationToken)
        {
            await _tenantCreationRequestService.EnableAutoRenewalAsync(@event.OrderId, @event.AutoRenewalIsEnabled, cancellationToken);
        }

    }
}
