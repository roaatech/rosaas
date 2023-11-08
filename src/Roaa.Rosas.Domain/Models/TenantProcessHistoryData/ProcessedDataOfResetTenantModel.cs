namespace Roaa.Rosas.Domain.Models.TenantProcessHistoryData
{
    public class ProcessedDataOfResetTenantModel : BaseProcessedDataOfTenant
    {
        public ProcessedDispatchedRequestModel DispatchedRequest { get; set; } = new();


        public ProcessedDataOfResetTenantModel(DispatchedRequestModel dispatchedRequest)
        {
            DispatchedRequest = Map(dispatchedRequest);
        }


        public override string Serialize()
        {
            return Serialize(this);
        }
    }
}



