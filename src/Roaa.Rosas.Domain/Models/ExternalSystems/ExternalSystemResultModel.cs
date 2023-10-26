namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public class ExternalSystemResultModel<T>
    {
        public T? Data { get; set; }
        public double DurationInMillisecond { get; set; }
        public string Url { get; set; } = string.Empty;
        public string SerializedResponseContent { get; set; } = string.Empty;
    }
}
