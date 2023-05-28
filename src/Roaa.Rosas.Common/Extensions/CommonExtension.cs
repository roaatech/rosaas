using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                var serialized = JsonConvert.SerializeObject(self);
                return JsonConvert.DeserializeObject<T>(serialized);
            }
            return self;
        }
    }
}
