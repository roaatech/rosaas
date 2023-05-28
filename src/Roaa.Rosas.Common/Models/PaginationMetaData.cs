namespace Roaa.Rosas.Common.Models
{
    public record PaginationMetaData
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        // public int Skip { get { return (Page - 1) * PageSize; } }



        public PaginationMetaData(int page, int pageSize)
        {
            Page = page > 0 ? page : 1;
            PageSize = pageSize > 0 ? pageSize : 15;
        }

        public PaginationMetaData()
        {
        }

        public int Skip()
        {
            return (Page - 1) * PageSize;
        }
    }
}
