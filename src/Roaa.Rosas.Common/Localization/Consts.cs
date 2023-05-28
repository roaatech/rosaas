namespace Roaa.Rosas.Common.Localization
{
    public class Constants
    {
        public const LanguageEnum DefaultLanguage = LanguageEnum.en;


        public static readonly IEnumerable<string> Languages = Enum.GetValues(typeof(LanguageEnum))
                                                                   .Cast<LanguageEnum>()
                                                                   .Select(x => x.ToString());

        public static readonly IEnumerable<LanguageEnum> Enumerations = Enum.GetValues(typeof(LanguageEnum))
                                                                            .Cast<LanguageEnum>();
    }
}
