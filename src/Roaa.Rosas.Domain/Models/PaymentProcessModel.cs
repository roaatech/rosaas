using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Models
{
    public class PaymentProcessModel : IPaymentProcess
    {
        public string? ProcessedPaymentId { get; set; }

        public string? AltProcessedPaymentId { get; set; }

        public string? ProcessedPaymentResult { get; set; }

        public string? CapturedPaymentResult { get; set; }

        public string? AuthorizedPaymentResult { get; set; }

        public string? ProcessedPaymentReference { get; set; }

        public string? ProcessedPaymentReferenceType { get; set; }
    }
}
