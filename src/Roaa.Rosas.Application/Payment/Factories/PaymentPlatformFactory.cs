using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Application.Payment.Platforms;
using Roaa.Rosas.Application.Payment.Platforms.ManwalService;
using Roaa.Rosas.Application.Payment.Platforms.StripeService;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Factories
{
    public class PaymentPlatformFactory : IPaymentPlatformFactory
    {

        #region Props          
        private readonly IServiceProvider _serviceProvider;

        #endregion


        #region Ctors
        public PaymentPlatformFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion


        public IPaymentPlatformService GetPaymentMethod(PaymentPlatform? paymentPlatform)
        {
            switch (paymentPlatform)
            {

                case PaymentPlatform.Stripe:
                    {
                        return _serviceProvider.GetService<StripePaymentPlatformService>();
                    }

                default:
                case null:
                case PaymentPlatform.Manwal:
                    {
                        return _serviceProvider.GetService<ManwalPaymentPlatformService>();
                    }
            }
        }

    }
}



