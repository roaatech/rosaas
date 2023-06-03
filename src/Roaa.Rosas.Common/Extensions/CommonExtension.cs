using Newtonsoft.Json;
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
    }
}
