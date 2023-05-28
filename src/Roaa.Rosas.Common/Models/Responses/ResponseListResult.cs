namespace Roaa.Rosas.Education.API.Models.Common.Responses
{
    public class ResponseListResult<T> : ResponseResult
    {
        public List<T>? Data { get; set; }
    }
}
