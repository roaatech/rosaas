using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class PaymentProcessingPreparationEvent : BaseInternalEvent
    {
        public Guid OrderId { get; set; }
        public bool AutoRenewalIsEnabled { get; set; }



        public PaymentProcessingPreparationEvent()
        {
        }


        public PaymentProcessingPreparationEvent(Guid orderId, bool autoRenewalIsEnabled)
        {
            OrderId = orderId;
            AutoRenewalIsEnabled = autoRenewalIsEnabled;
        }
    }
}
