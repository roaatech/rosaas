using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Settings
{
    public class TenantSettings : ISettings
    {
        public bool SendCreationRequestAutomaticallyAfterTenantCreatedInStore { get; set; } = true;
        public List<TenancyType> TenancyTypes { get; set; } = new List<TenancyType> { TenancyType.Planed };
    }
}
