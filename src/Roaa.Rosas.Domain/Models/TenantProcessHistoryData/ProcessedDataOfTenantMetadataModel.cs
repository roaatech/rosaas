namespace Roaa.Rosas.Domain.Models.TenantProcessHistoryData
{
    public class ProcessedDataOfTenantMetadataModel : BaseProcessedDataOfTenant
    {

        public dynamic UpdatedData { get; set; } = string.Empty;

        public dynamic OldData { get; set; } = string.Empty;

        public ProcessedDataOfTenantMetadataModel(dynamic updatedData, string oldData)
        {
            UpdatedData = updatedData;

            OldData = string.IsNullOrWhiteSpace(oldData) ? null : System.Text.Json.JsonSerializer.Deserialize<dynamic>(oldData);
        }


        public override string Serialize()
        {
            return Serialize(this);
        }
    }
}
