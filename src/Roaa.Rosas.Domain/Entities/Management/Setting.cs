namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Setting : BaseEntity
    {
        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return Key;
        }
        public string ToPropertyName()
        {
            return Key.Substring(Key.LastIndexOf('.') + 1);
        }
    }
}
