using Roaa.Rosas.Common.Models;
using System.ComponentModel;
using System.Globalization;

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


        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }


        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (destinationType.IsEnum && value is not int)
                return Enum.Parse(destinationType, value.ToString(), true);

            var valueStr = TypeDescriptor.GetConverter(destinationType).ConvertToInvariantString(value);
            var sourceType = value.GetType();

            var destinationConverter = TypeDescriptor.GetConverter(destinationType);

            if (destinationConverter.CanConvertFrom(value.GetType()))
                return destinationConverter.ConvertFrom(null, culture, value);

            var sourceConverter = TypeDescriptor.GetConverter(sourceType);

            if (sourceConverter.CanConvertTo(destinationType))
                return sourceConverter.ConvertTo(null, culture, value, destinationType);

            if (destinationType.IsEnum && value is int)
                return Enum.ToObject(destinationType, (int)value);

            if (!destinationType.IsInstanceOfType(value))
                return Convert.ChangeType(value, destinationType, culture);

            return value;
        }


        public static T To<T>(object value)
        {
            return (T)To(value, typeof(T));
        }
    }
}
