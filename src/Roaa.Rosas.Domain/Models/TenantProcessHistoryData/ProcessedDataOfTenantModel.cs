namespace Roaa.Rosas.Domain.Models.TenantProcessHistoryData
{
    public class ProcessedDataOfTenantModel : BaseProcessedDataOfTenant
    {
        public List<ProcessedTenantPropertyValueModel> Properties { get; set; }
        public ProcessedDataOfTenantModel(List<ProcessedTenantPropertyValueModel> properties)
        {
            Properties = properties;
        }
        public ProcessedDataOfTenantModel(params ProcessedTenantPropertyValueModel[] properties)
        {
            Properties = properties.ToList();
        }
        public override string Serialize()
        {
            return Serialize(this);
        }

    }
    public class ProcessedTenantPropertyValueModel
    {
        public string Name { get; set; } = string.Empty;
        public string PreviousValue { get; set; } = string.Empty;
        public string UpdatedValue { get; set; } = string.Empty;
    }



}


