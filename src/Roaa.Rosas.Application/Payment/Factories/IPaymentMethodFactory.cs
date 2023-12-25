using Roaa.Rosas.Application.Payment.Methods;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Factories
{
    public interface IPaymentMethodFactory
    {
        IPaymentMethodService GetPaymentMethod(PaymentMethodType? type);
    }
}



