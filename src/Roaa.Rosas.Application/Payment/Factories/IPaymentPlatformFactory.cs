using Roaa.Rosas.Application.Payment.Platforms;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Factories
{
    public interface IPaymentPlatformFactory
    {
        IPaymentPlatformService GetPaymentMethod(PaymentPlatform? paymentPlatform);
    }
}



