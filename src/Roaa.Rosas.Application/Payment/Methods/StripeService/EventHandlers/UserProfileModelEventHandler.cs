using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Payment.Methods.StripeService.EventHandlers
{
    public class UserProfileModelEventHandler : IInternalDomainEventHandler<UserProfileModelEvent>
    {
        private readonly ILogger<UserProfileModelEventHandler> _logger;
        private readonly IStripePaymentMethodService _stripePaymentMethodService;


        public UserProfileModelEventHandler(IStripePaymentMethodService stripePaymentMethodService,
                                                     ILogger<UserProfileModelEventHandler> logger)
        {
            _stripePaymentMethodService = stripePaymentMethodService;
            _logger = logger;
        }


        public async Task Handle(UserProfileModelEvent @event, CancellationToken cancellationToken)
        {
            await _stripePaymentMethodService.UpdateCustomerAsync(@event.UpdatedProfile.FullName, @event.UpdatedProfile.MobileNumber, @event.UserId, cancellationToken);
        }
    }
}
