using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Common.Utilities
{
    public class Helpers
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static bool TryParseEnum<T>(string value, out T? @enum)
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            @enum = default(T);
            if (values.Any(x => x.ToString().Equals(value)))
            {
                @enum = values.Where(x => x.ToString().Equals(value)).FirstOrDefault();
                return true;
            }
            return false;
        }

        public static List<FilterItem> ConvertToFilterItem(DateTime? fromDate, DateTime? toDate)
        {
            List<FilterItem> filters = null;

            if (fromDate.HasValue)
            {
                filters = filters ?? new List<FilterItem>();

                filters.Add(new FilterItem
                {
                    Field = "fromDate",
                    Value = fromDate.Value.ToString(),
                    Operator = FilterOperator.GreaterThanOrEqual,
                });
            }
            if (toDate.HasValue)
            {
                filters = filters ?? new List<FilterItem>();

                filters.Add(new FilterItem
                {
                    Field = "toDate",
                    Value = toDate.Value.ToString(),
                    Operator = FilterOperator.LessThanOrEqual,
                });
            }

            return filters;
        }
    }
}
