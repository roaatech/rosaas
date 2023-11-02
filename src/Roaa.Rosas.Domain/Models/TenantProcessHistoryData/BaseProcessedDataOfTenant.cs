namespace Roaa.Rosas.Domain.Models.TenantProcessHistoryData
{
    public abstract class BaseProcessedDataOfTenant
    {
        public abstract string Serialize();
        public string Serialize(dynamic data)
        {
            return System.Text.Json.JsonSerializer.Serialize(data);
        }
        public ProcessedDispatchedRequestModel? Map(DispatchedRequestModel? dispatchedRequest)
        {
            if (dispatchedRequest is null) return null;

            return new ProcessedDispatchedRequestModel
            {
                DurationInMillisecond = dispatchedRequest.DurationInMillisecond,
                RequestUrl = dispatchedRequest.Url,
                ResponseContent = string.IsNullOrWhiteSpace(dispatchedRequest.SerializedResponseContent) ? null : System.Text.Json.JsonSerializer.Deserialize<dynamic>(dispatchedRequest.SerializedResponseContent),
            };
        }
    }
}
