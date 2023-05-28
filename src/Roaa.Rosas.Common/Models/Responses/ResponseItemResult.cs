namespace Roaa.Rosas.Education.API.Models.Common.Responses
{
    public class ResponseItemResult<T> : ResponseResult
    {
        public T? Data { get; set; }
    }
}
