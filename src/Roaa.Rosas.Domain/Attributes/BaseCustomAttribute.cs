namespace Roaa.Rosas.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BaseCustomAttribute : Attribute
    {
        public string TypeAsString { get; set; }

        public BaseCustomAttribute(string typeAsString)
        {
            TypeAsString = typeAsString;
        }
    }
}
