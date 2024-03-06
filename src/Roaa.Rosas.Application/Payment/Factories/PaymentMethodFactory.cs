using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Application.Payment.Methods;
using Roaa.Rosas.Application.Payment.Methods.ManwalService;
using Roaa.Rosas.Application.Payment.Methods.StripeService;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Factories
{
    public class PaymentMethodFactory : IPaymentMethodFactory
    {

        #region Props          
        private readonly IServiceProvider _serviceProvider;

        #endregion


        #region Ctors
        public PaymentMethodFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion


        public IPaymentMethodService GetPaymentMethod(PaymentPlatform? paymentPlatform)
        {
            switch (paymentPlatform)
            {

                case PaymentPlatform.Stripe:
                    {
                        return _serviceProvider.GetService<StripePaymentMethodService>();
                    }

                default:
                case null:
                case PaymentPlatform.Manwal:
                    {
                        return _serviceProvider.GetService<ManwalPaymentMethodService>();
                    }
            }
        }

    }
}



