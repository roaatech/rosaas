using FluentValidation;


namespace Roaa.Rosas.Application.Extensions
{
    public static class ValidatorExtension
    {
        public static IRuleBuilderOptions<T, int> IsInEnumRangeExceptValue<T>(this IRuleBuilderInitial<T, int> ruleBuilder, Type enumType, int exceptedValue)
        {
            var arr = Enum.GetValues(enumType).Cast<int>().ToHashSet();
            return ruleBuilder.Must(x => arr.Any(a => a == x && x != exceptedValue));
        }

        public static IRuleBuilderOptions<T, int> IsInEnumRange<T>(this IRuleBuilderInitial<T, int> ruleBuilder, Type enumType)
        {
            var arr = Enum.GetValues(enumType).Cast<int>().ToHashSet();
            return ruleBuilder.Must(x => arr.Any(a => a == x));
        }

        public static bool IsEmptyGuid(this Guid guid)
        {
            return guid == Guid.Empty;
        }
    }
}
