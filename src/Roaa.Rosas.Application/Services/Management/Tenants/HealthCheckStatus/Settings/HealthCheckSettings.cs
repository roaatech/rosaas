using Roaa.Rosas.Application.Interfaces;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Settings
{
    public class HealthCheckSettings : ISettings
    {
        public int AvailableCheckTimePeriod { get; set; } = 15;
        public int InaccessibleCheckTimePeriod { get; set; } = 1;
        public int UnavailableCheckTimePeriod { get; set; } = 2;
        public int TimesNumberBeforeInformExternalSys { get; set; } = 3;
    }
}

