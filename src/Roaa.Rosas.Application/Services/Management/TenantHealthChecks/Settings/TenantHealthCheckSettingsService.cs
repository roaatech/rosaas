using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.TenantHealthChecks.Settings
{
    public partial class TenantHealthCheckSettingsService : ITenantHealthCheckSettingsService
    {
        #region Props 
        private readonly ILogger<TenantService> _logger;
        private readonly ISettingService _settingService;
        protected readonly BackgroundServicesStore _backgroundWorkerStore;
        protected readonly IAvailableTenantChecker _availableTenantChecker;
        protected readonly IInaccessibleTenantChecker _inaccessibleTenantChecker;
        protected readonly IUnavailableTenantChecker _unavailableTenantChecker;
        #endregion

        #region Corts
        public TenantHealthCheckSettingsService(ILogger<TenantService> logger,
                ISettingService settingService,
                BackgroundServicesStore backgroundWorkerStore,
                IAvailableTenantChecker availableTenantChecker,
                IInaccessibleTenantChecker inaccessibleTenantChecker,
                IUnavailableTenantChecker unavailableTenantChecker)
        {
            _logger = logger;
            _settingService = settingService;
            _backgroundWorkerStore = backgroundWorkerStore;
            _availableTenantChecker = availableTenantChecker;
            _inaccessibleTenantChecker = inaccessibleTenantChecker;
            _unavailableTenantChecker = unavailableTenantChecker;
        }

        #endregion


        #region Services    

        public async Task<Result<HealthCheckSettings>> GetTenantHealthCheckSettingsAsync(CancellationToken cancellationToken = default)
        {
            return await _settingService.LoadSettingAsync<HealthCheckSettings>(cancellationToken);
        }




        public async Task<Result> UpdateTenantHealthCheckSettingsAsync([FromBody] HealthCheckSettings model, CancellationToken cancellationToken = default)
        {
            var result = await _settingService.SaveSettingAsync(model, cancellationToken);

            if (result.Success)
            {
                _backgroundWorkerStore.SetHealthCheckSettings(model);
                await _availableTenantChecker.RestartAsync(cancellationToken);
                await _inaccessibleTenantChecker.RestartAsync(cancellationToken);
                await _unavailableTenantChecker.RestartAsync(cancellationToken);
            }
            return result;
        }
        #endregion
    }
}
