using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Payment.Platforms.StripeService;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Payment.Platforms.StripeService.EventHandlers
{
    public class UserProfileModelEventHandler : IInternalDomainEventHandler<UserProfileModelEvent>
    {
        private readonly ILogger<UserProfileModelEventHandler> _logger;
        private readonly IStripePaymentPlatformService _stripePaymentMethodService;


        public UserProfileModelEventHandler(IStripePaymentPlatformService stripePaymentMethodService,
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
