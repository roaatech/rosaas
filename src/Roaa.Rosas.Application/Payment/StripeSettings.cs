using Roaa.Rosas.Domain.Settings;

namespace Roaa.Rosas.Application.Payment
{
    public class StripeSettings : ISettings
    {
        public string TestApiKey { get; set; } = "sk_test_51OI5l1E1TtPRg7pa4T6AXcmiCjV9LfEig2glaELNmdTZPiurAvScKg1jjE60z3hIKoFzrVBvVQwOCsA1J1SHnyIe00cmlLZnso";
        public string LiveApiKey { get; set; } = string.Empty;
        public bool TestMode { get; set; } = true;
        public bool FailedMode { get; set; } = false;
        public bool PassProductNamesAndTotals { get; set; }
        public decimal AdditionalFee { get; set; } = 1;
        public bool AdditionalFeePercentage { get; set; } = true;

    }





}



