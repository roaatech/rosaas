namespace Roaa.Rosas.Common.Models
{
    public class SortItem
    {
        public string Field { get; set; } = string.Empty;
        public SortDirection Direction { get; set; }
    }

    public enum SortDirection
    {
        Asc = 1,
        Desc = 2,
    }
}
