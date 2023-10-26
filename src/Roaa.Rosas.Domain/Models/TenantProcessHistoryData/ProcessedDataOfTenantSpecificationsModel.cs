namespace Roaa.Rosas.Domain.Models.TenantProcessHistoryData
{
    public class ProcessedDataOfTenantSpecificationsModel : BaseProcessedDataOfTenant
    {

        public List<ProcessedTenantSpecificationValueModel> Specifications { get; set; }
        public ProcessedDataOfTenantSpecificationsModel(List<ProcessedTenantSpecificationValueModel> specifications)
        {
            Specifications = specifications;
        }

        public override string Serialize()
        {
            return Serialize(this);
        }
    }


    public class ProcessedTenantSpecificationValueModel
    {
        public string Name { get; set; } = string.Empty;
        public string PreviousValue { get; set; } = string.Empty;
        public string UpdatedValue { get; set; } = string.Empty;
    }
}
