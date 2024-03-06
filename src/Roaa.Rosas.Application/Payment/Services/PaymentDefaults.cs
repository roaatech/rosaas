using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Constatns
{
    public partial class Consts
    {
        public class PaymentDefaults
        {
            public const PaymentMethodType DefaultPaymentMethod = PaymentMethodType.Card;
            public const PaymentPlatform DefaultPaymentPlatform = PaymentPlatform.Manwal;
        }
    }
}



