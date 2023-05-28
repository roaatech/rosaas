using Roaa.Rosas.Common.Models.ResponseMessages;

namespace Roaa.Rosas.Common.Models.Results
{
    public class PaginatedResult<T> : Result
    {
        public PaginatedResult(IEnumerable<T> data, int totalCount)
        {
            Data = data;
            TotalCount = totalCount;
        }
        public PaginatedResult()
        {
        }

        public IEnumerable<T> Data { get; internal set; } = new List<T>();
        public int TotalCount { get; internal set; }

        public new static PaginatedResult<T> Fail(string error)
        {
            return new PaginatedResult<T>() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(error) } };
        }

        public static PaginatedResult<T> Successful(IEnumerable<T> data, int totalCount)
        {
            return new PaginatedResult<T>() { Success = true, Data = data, TotalCount = totalCount };
        }

    }
}
