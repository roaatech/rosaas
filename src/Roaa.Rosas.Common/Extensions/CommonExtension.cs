using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Reflection;

namespace Roaa.Rosas.Common.Extensions
{
    public static class CommonExtension
    {

        public static bool IsDerivedFrom(this Type derived, Type @base)
        {
            //@base assignable from derived
            return @base.GetTypeInfo().IsAssignableFrom(derived.Ge‌​tTypeInfo());
        }

        public static T DeepCopy<T>(this T self)
        {
            if (self is not null)
            {
                var serialized = JsonConvert.SerializeObject(self, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                return JsonConvert.DeserializeObject<T>(serialized);

            }
            return self;
        }


        public static string ToSnakeCaseNamingStrategy(this Enum name)
        {
            return name.ToString().ToSnakeCaseNamingStrategy();
        }
        public static string ToSnakeCaseNamingStrategy(this string propertName)
        {
            return new SnakeCaseNamingStrategy().GetPropertyName(propertName, false);
        }
        public static string ToPascalCaseNamingStrategy(this string propertName)
        {
            var yourString = propertName.ToLower().Replace("_", " ");
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(yourString).Replace(" ", string.Empty);
        }
    }
}
