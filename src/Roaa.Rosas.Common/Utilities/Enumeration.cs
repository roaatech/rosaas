using System.Reflection;

namespace Roaa.Rosas.Common.Utilities
{
    public abstract class Enumeration<TEnum, Tkey> : IEquatable<Enumeration<TEnum, Tkey>> where TEnum : Enumeration<TEnum, Tkey> where Tkey : struct
    {

        private static readonly Dictionary<Tkey, TEnum> Enumerations = CreateEnumerations();

        protected Enumeration(Tkey key) => Key = key;

        public Tkey Key { get; protected set; } = default;

        public static TEnum FromKey(Tkey key)
        {
            var enumeration = Fromkey(key);
            if (enumeration is null)
            {
                throw new NullReferenceException($"{typeof(TEnum)} - {typeof(Tkey)}");
            }
            return enumeration;
        }
        private static TEnum? Fromkey(Tkey key)
        {
            return Enumerations.TryGetValue(key, out TEnum? enumeration) ?
                enumeration : default;
        }
        public bool Equals(Enumeration<TEnum, Tkey>? other)
        {
            if (other is null)
            {
                return false;
            }

            return GetType() == other.GetType() && Key.Equals(other.Key);
        }

        public override string ToString() => Key.ToString();

        public override int GetHashCode() => Key.GetHashCode();

        public static Dictionary<Tkey, TEnum> CreateEnumerations()
        {
            var enumrationType = typeof(TEnum);
            var fielsForType = enumrationType
                    .GetFields(BindingFlags.Public |
                               BindingFlags.Static |
                               BindingFlags.DeclaredOnly)
                    .Where(fieldInfo =>
                        enumrationType.IsAssignableFrom(fieldInfo.FieldType))
                    .Select(fieldInfo =>
                        (TEnum)fieldInfo.GetValue(default)!);
            return fielsForType.ToDictionary(x => x.Key);
        }
    }

}
