using System; 

namespace Roaa.Rosas.Common.Localization
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class LocalizationAttribute : Attribute, ILocalizedString
    {
        public string En { get; set; } = string.Empty;
        public string Ar { get; set; } = string.Empty;
    }
}
