using MediatR;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.EventHandlers.EventHandlers
{
    public class SubscriptionRenewalHasBeenDisabledEventHandler : IInternalDomainEventHandler<SubscriptionRenewalHasBeenDisabledEvent>
    {

        #region  
        private readonly IPublisher _publisher;
        private readonly IInstanceFactory<SubscriptionRenewalHasBeenDisabledBaseEvent, SubscriptionRenewalTypeAttribute> _instanceFactory;
        #endregion


        #region Corts
        public SubscriptionRenewalHasBeenDisabledEventHandler(IInstanceFactory<SubscriptionRenewalHasBeenDisabledBaseEvent, SubscriptionRenewalTypeAttribute> instanceFactory,
                                                               IPublisher publisher)
        {
            _publisher = publisher;
            _instanceFactory = instanceFactory;
        }
        #endregion



        public async Task Handle(SubscriptionRenewalHasBeenDisabledEvent @event, CancellationToken cancellationToken)
        {
            // An instance of the event is created more accurately according to the type of subscription renewal and its triggering
            // We implemented a try/catch statement to ensure that any issues in publishing the event do not affect the main business process.
            try
            {
                var tenantAvailabilityChangedEvent = _instanceFactory.CreateInstance(@event.SubscriptionRenewal.Type.ToString());

                tenantAvailabilityChangedEvent.SubscriptionRenewal = @event.SubscriptionRenewal;

                await _publisher.Publish(tenantAvailabilityChangedEvent);
            }
            catch (Exception ex) { }
        }
    }
}
