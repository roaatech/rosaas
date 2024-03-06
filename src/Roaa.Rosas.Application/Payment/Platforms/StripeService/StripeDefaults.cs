namespace Roaa.Rosas.Application.Constatns
{
    public partial class Consts
    {
        public class StripeDefaults
        {
            public const string RoSaasUserId = "rosaas-user-id";
            public const string RoSaasOrderId = "rosaas-order-id";


            public const string PaymentIntentAuthorizedStatus = "requires_capture";
            public const string PaymentIntentPaidStatus = "succeeded";

            public const string SessionPendingPaymentStatus = "unpaid";
            public const string SessionPendingStatus = "open";
            public const string SessionPaidPaymentStatus = "paid";
            public const string SessionPaidStatus = "complete";
            public const string SessionAuthorizedPaymentStatus = "unpaid";
            public const string SessionAuthorizedStatus = "complete";
        }
    }
}
