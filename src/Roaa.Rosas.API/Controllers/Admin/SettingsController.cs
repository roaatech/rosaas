using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Settings;
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
        private readonly ITenantHealthCheckSettingsService _settingService;

        #endregion

        #region Corts
        public SettingsController(ILogger<SettingsController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                                 ITenantHealthCheckSettingsService settingService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _settingService = settingService;
        }
        #endregion

        #region Actions   


        [HttpGet("HealthCheck")]
        public async Task<IActionResult> GetHealthCheckSettingsAsync(CancellationToken cancellationToken = default)
        {
            return Ok(await _settingService.GetTenantHealthCheckSettingsAsync(cancellationToken));
        }





        [HttpPut("HealthCheck")]
        public async Task<IActionResult> UpdateHealthCheckSettingsAsync([FromBody] HealthCheckSettings model, CancellationToken cancellationToken = default)
        {
            await _settingService.UpdateTenantHealthCheckSettingsAsync(model, cancellationToken);

            return EmptyResult();
        }
        #endregion


    }


}
