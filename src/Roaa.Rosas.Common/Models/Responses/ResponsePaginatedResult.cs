namespace Roaa.Rosas.Education.API.Models.Common.Responses
{
    public class ResponsePaginatedResult<T> : ResponseResult
    {
        public ResponsePaginatedModel<T> Data { get; set; } = new();
    }

    public class ResponsePaginatedModel<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
    }
}
