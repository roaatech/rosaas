using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment
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


        public IPaymentMethod GetPaymentMethod(PaymentMethodType type)
        {
            switch (type)
            {
                default:
                case PaymentMethodType.Stripe:
                    {
                        return _serviceProvider.GetService<StripePaymentMethod>();
                    }
            }
        }

    }





}



