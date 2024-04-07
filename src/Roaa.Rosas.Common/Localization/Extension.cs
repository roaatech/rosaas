using System.Reflection;

namespace Roaa.Rosas.Common.Localization
{
    public static class Extension
    {
        public static string Localize(this Enum key, LanguageEnum locale)
        {
            var attribute = key.GetType()
                               .GetMember(key.ToString())[0]
                               .GetCustomAttributes<LocalizationAttribute>(false)
                               .FirstOrDefault();

            var msg = locale switch
            {
                LanguageEnum.ar => attribute?.Ar,
                LanguageEnum.en => attribute?.En,
                _ => throw new NotImplementedException($@"Can not localize a key ({key.ToString()}) of {key.GetType().Name}, So you must bind ({locale}) locale.")
            };

            return msg ?? string.Empty;
        }

        public static LanguageEnum ToLanguageOrDefault(this string locale)
        {
            if (string.IsNullOrWhiteSpace(locale)) return Constants.DefaultLanguage;
            return Enum.TryParse(locale, out LanguageEnum language) ? language : Constants.DefaultLanguage;
        }
    }
}
