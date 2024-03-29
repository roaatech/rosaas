﻿using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.BackgroundServices;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Settings;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    [Authorize(Policy = AuthPolicy.Management.Settings, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class SettingsController : BaseManagementApiController
    {
        #region Props 
        private readonly ILogger<SettingsController> _logger;
        private readonly ITenantHealthCheckSettingsService _healthCheckSettingService;
        private readonly ISettingService _settingService;
        private readonly ISubscriptionWorker _subscriptionWorker;

        #endregion

        #region Corts
        public SettingsController(ILogger<SettingsController> logger,
                                 ITenantHealthCheckSettingsService healthCheckSettingService,
                                 ISettingService settingService,
                                 ISubscriptionWorker subscriptionWorker)
        {
            _logger = logger;
            _healthCheckSettingService = healthCheckSettingService;
            _settingService = settingService;
            _subscriptionWorker = subscriptionWorker;
        }
        #endregion

        #region Actions   

        #region Health Check  
        [HttpGet("HealthCheck")]
        public async Task<IActionResult> GetHealthCheckSettingsAsync(CancellationToken cancellationToken = default)
        {
            return Ok(await _healthCheckSettingService.GetTenantHealthCheckSettingsAsync(cancellationToken));
        }


        [HttpPut("HealthCheck")]
        public async Task<IActionResult> UpdateHealthCheckSettingsAsync([FromBody] HealthCheckSettings model, CancellationToken cancellationToken = default)
        {
            await _healthCheckSettingService.UpdateTenantHealthCheckSettingsAsync(model, cancellationToken);

            return EmptyResult();
        }
        #endregion


        #region Subscription     
        [HttpGet("Subscription")]
        public async Task<IActionResult> GetSubscriptionSettingsAsync(CancellationToken cancellationToken = default)
        {
            return Ok(await _settingService.LoadSettingAsync<SubscriptionSettings>(cancellationToken));
        }



        [HttpPut("Subscription")]
        public async Task<IActionResult> UpdateSubscriptionSettingsAsync([FromBody] SubscriptionSettings model, CancellationToken cancellationToken = default)
        {
            var result = await _settingService.SaveSettingAsync(model, cancellationToken);

            if (result.Success)
            {
                await _subscriptionWorker.RestartAsync(cancellationToken);
            }

            return EmptyResult();
        }
        #endregion


        #region Tenant     
        [HttpGet("Tenant")]
        public async Task<IActionResult> GetTenantSettingsAsync(CancellationToken cancellationToken = default)
        {
            return Ok(await _settingService.LoadSettingAsync<TenantSettings>(cancellationToken));
        }


        [HttpPut("Tenant")]
        public async Task<IActionResult> UpdateTenantSettingsAsync([FromBody] TenantSettings model, CancellationToken cancellationToken = default)
        {
            var result = await _settingService.SaveSettingAsync(model, cancellationToken);

            if (result.Success)
            {
                await _subscriptionWorker.RestartAsync(cancellationToken);
            }

            return EmptyResult();
        }
        #endregion


        #region Tenant     
        [HttpGet("Product/Warnings")]
        public async Task<IActionResult> GetProductWarningsSettingsAsync(CancellationToken cancellationToken = default)
        {
            return Ok(await _settingService.LoadSettingAsync<ProductWarningsSettings>(cancellationToken));
        }


        [HttpPut("Product/Warnings")]
        public async Task<IActionResult> UpdateProductWarningsSettingsAsync([FromBody] ProductWarningsSettings model, CancellationToken cancellationToken = default)
        {
            var result = await _settingService.SaveSettingAsync(model, cancellationToken);

            return EmptyResult();
        }
        #endregion

        #endregion
    }


}
