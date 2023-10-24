namespace Roaa.Rosas.Domain.Settings
{
    public class TenantSettings : ISettings
    {
        public bool SendCreationRequestAutomaticallyAfterTenantCreatedInStore { get; set; } = true;
    }
}
