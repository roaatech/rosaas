using System.Net;

namespace Roaa.Rosas.RequestBroker.Models
{
    public class RequestResult<T>
    {
        public T Data { get; set; }

        public bool Success { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public double DurationInMillisecond { get; set; }

        public Dictionary<string, List<string>> Errors { get; set; }
    }
}
