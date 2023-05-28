using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Common.Extensions
{
    public static class SortExtensions
    {
        public static bool IsDesc(this SortDirection direction)
        {
            return direction == SortDirection.Desc;
        }
        public static SortItem HandleDefaultSorting(this SortItem sort, string[] sortingColumns, string defaultSorting, SortDirection defaultDirection)
        {
            if (sort is null || string.IsNullOrWhiteSpace(sort.Field) || !sortingColumns.Any(x => x.Equals(sort.Field, StringComparison.OrdinalIgnoreCase)))
            {
                sort = new SortItem
                {
                    Field = defaultSorting,
                    Direction = defaultDirection,
                };
                return sort;
            }

            var result = new SortItem
            {
                Field = sortingColumns.First(x => x.Equals(sort.Field, StringComparison.OrdinalIgnoreCase)),
                Direction = sort.Direction
            };

            sort = result;
            return result;
        }
    }
}
