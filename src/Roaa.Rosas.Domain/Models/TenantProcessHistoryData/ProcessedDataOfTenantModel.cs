namespace Roaa.Rosas.Domain.Models.TenantProcessHistoryData
{
    public class ProcessedDataOfTenantModel : BaseProcessedDataOfTenant
    {

        public ProcessedTenantModel UpdatedData { get; set; } = new(string.Empty);

        public ProcessedTenantModel OldData { get; set; } = new(string.Empty);


        public ProcessedDataOfTenantModel(ProcessedTenantModel updatedData, ProcessedTenantModel oldData)
        {
            UpdatedData = updatedData;
            OldData = oldData;
        }

        public override string Serialize()
        {
            return Serialize(this);
        }

    }
    public class ProcessedTenantModel
    {


        public string Title { get; set; } = string.Empty;

        public ProcessedTenantModel(string title)
        {
            Title = title;
        }
    }
}
