namespace Roaa.Rosas.Domain.Models.TenantProcessHistoryData
{
    public class ProcessedDataOfSubscriptionFeatureLimitResetModel : BaseProcessedDataOfTenant
    {
        public List<string> Features { get; set; } = new();


        public ProcessedDataOfSubscriptionFeatureLimitResetModel(params string[] features)
        {
            Features = features.ToList();
        }

        public ProcessedDataOfSubscriptionFeatureLimitResetModel(IEnumerable<string> features)
        {
            Features = features.ToList();
        }


        public override string Serialize()
        {
            return Serialize(this);
        }
    }

}
