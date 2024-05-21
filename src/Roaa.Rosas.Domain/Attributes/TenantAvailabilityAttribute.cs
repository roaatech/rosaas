namespace Roaa.Rosas.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TenantAvailabilityAttribute : BaseCustomAttribute
    {
        public bool IsHealthy { get; set; }

        public TenantAvailabilityAttribute(bool isHealthy) : base(isHealthy.ToString())
        {
            IsHealthy = isHealthy;
        }
    }
}
