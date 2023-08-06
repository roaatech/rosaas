using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    public class SettingsController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<SettingsController> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;

        #endregion

        #region Corts
        public SettingsController(ILogger<SettingsController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
        }
        #endregion

        #region Actions   

        [HttpGet("HealthCheck")]
        public async Task<IActionResult> GetHealthCheckSettingsAsync(CancellationToken cancellationToken = default)
        {
            return ItemResult(HealthCheckSettings.Settings);
        }





        [HttpPut("HealthCheck")]
        public async Task<IActionResult> UpdateHealthCheckSettingsAsync([FromBody] HealthCheckSettings model, CancellationToken cancellationToken = default)
        {
            HealthCheckSettings.Settings = model;
            return EmptyResult();
        }

        #endregion


    }

    public class HealthCheckSettings
    {
        public int AvailableCheckTimePeriod { get; set; }
        public int InaccessibleCheckTimePeriod { get; set; }
        public int UnavailableCheckTimePeriod { get; set; }
        public int TimesNumberBeforeInformExternalSys { get; set; }

        public static HealthCheckSettings Settings = new HealthCheckSettings();
    }
}
