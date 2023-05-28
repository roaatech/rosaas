using FluentValidation;
using Roaa.Rosas.Common.Localization;
using System.Data;


namespace Roaa.Rosas.Common.Extensions
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

        public static IRuleBuilderOptions<T, List<I>> ContainsLeastOneItem<T, I>(this IRuleBuilderInitial<T, List<I>> ruleBuilder)
        {
            return ruleBuilder.Must(list => list is not null && list.Count > 0).WithMessage("Must contains at least one item");
        }


        public static IRuleBuilderOptions<T, List<I>> ContainsLeastItems<T, I>(this IRuleBuilderInitial<T, List<I>> ruleBuilder, int itemsCount)
        {
            return ruleBuilder.Must(list => list is not null && list.Count > itemsCount - 1).WithMessage($"Must contains at least {itemsCount} items");
        }


        public static bool IsEmptyGuid(this Guid guid)
        {
            return guid == Guid.Empty;
        }
        public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Enum error, LanguageEnum locale)
        {
            rule.WithErrorCode(Convert.ToInt32(error).ToString())
                .WithMessage(error.Localize(locale));
            return rule;
        }


        public static IRuleBuilderOptions<T, DateTime?> NotNullOrDefault<T, DateTime>(this IRuleBuilderInitial<T, DateTime?> ruleBuilder)
        {
            return ruleBuilder.Must(x => x != null && !x.Equals(default(DateTime)));
        }

    }
}
