namespace Roaa.Rosas.Domain.Models
{
    public class DispatchedRequestModel
    {
        public double DurationInMillisecond { get; set; }
        public string Url { get; set; } = string.Empty;
        public string SerializedResponseContent { get; set; } = string.Empty;


        public DispatchedRequestModel(double durationInMillisecond, string url, string serializedResponseContent)
        {
            DurationInMillisecond = durationInMillisecond;
            Url = url;
            SerializedResponseContent = serializedResponseContent;
        }
    }

    public class ReceivedRequestModel
    {
        public dynamic? RequestBody { get; set; }

        public ReceivedRequestModel(dynamic requestBody)
        {
            RequestBody = requestBody;
        }

        public ReceivedRequestModel()
        {
        }
    }
}
