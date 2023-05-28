namespace Roaa.Rosas.Common.Models
{
    public class FilterItem
    {
        public string Field { get; set; } = string.Empty;

        public FilterOperator Operator { get; set; }

        public string Value { get; set; } = string.Empty;
    }

    public enum FilterOperator
    {
        Equal = 1,
        Contains = 2,
        GreaterThanOrEqual,
        LessThanOrEqual
    }

}
