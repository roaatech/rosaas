using System;
using System.Linq;
using System.Reflection;

namespace Roaa.Rosas.Common.Localization
{
    public static class Extension
    {
        public static string Localize(this ILocalizedString localizedString, string locale)
        {
            return localizedString.Localize(locale.ToLanguageOrDefault());
        } 

        public static string Localize(this ILocalizedString localizedString, LanguageEnum locale)
        {
            if (localizedString == null)
                return string.Empty;

            string value = (string)localizedString.GetLocalizedValue(locale);
            if (!string.IsNullOrWhiteSpace(value) || locale == Constants.DefaultLanguage)
                return value;

            return (string)localizedString.GetLocalizedValue(Constants.DefaultLanguage);
        }

        public static string Localize(this Enum key, LanguageEnum locale)
        {
            var att = (LocalizationAttribute)key.GetType().GetMember(key.ToString())[0].GetCustomAttributes(typeof(LocalizationAttribute), false).FirstOrDefault();
            return att.Localize(locale) ?? string.Empty;
        } 

        private static string GetLocalizedValue(this ILocalizedString src, LanguageEnum propName)
        {
            return (string)src.GetType()
                              .GetProperty(propName.ToString(),
                                           BindingFlags.IgnoreCase |
                                           BindingFlags.Public |
                                           BindingFlags.Instance)
                              .GetValue(src, null);
        } 

        public static LanguageEnum ToLanguageOrDefault(this string locale)
        {
            if (string.IsNullOrWhiteSpace(locale)) return Constants.DefaultLanguage;
            return Enum.TryParse(locale, out LanguageEnum language) ? language : Constants.DefaultLanguage;
        } 
    }
}
