﻿using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.HealthCheckStatus.Settings
{
    public interface ITenantHealthCheckSettingsService
    {
        Task<Result<HealthCheckSettings>> GetTenantHealthCheckSettingsAsync(CancellationToken cancellationToken = default);
        Task<Result> UpdateTenantHealthCheckSettingsAsync([FromBody] HealthCheckSettings model, CancellationToken cancellationToken = default);
    }
}
