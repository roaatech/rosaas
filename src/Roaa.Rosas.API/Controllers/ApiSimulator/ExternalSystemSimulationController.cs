﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Controllers;

namespace Roaa.Rosas.Framework.Controllers.ApiSimulator
{
    [Route("external-system-simulator/tenants")]
    public class ExternalSystemSimulationController : BaseApiController
    {
        #region Props  
        private readonly ISender _mediator;
        private readonly IIdentityContextService _identityContextService;

        #endregion

        #region Corts
        public ExternalSystemSimulationController(ISender mediator,
                                 IIdentityContextService identityContextService
                                 )
        {
            _identityContextService = identityContextService;
            _mediator = mediator;
        }
        #endregion


        #region Actions    
        [HttpPost("cerated")]
        public async Task<IActionResult> CreateTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }


        [HttpPost("active")]
        public async Task<IActionResult> ActivateTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }


        [HttpPost("inactive")]
        public async Task<IActionResult> DeactivateTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }


        [HttpPost("deleted")]
        public async Task<IActionResult> DeleteTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }


        [HttpPost("reset")]
        public async Task<IActionResult> ResetTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }


        [HttpPost("upgrade")]
        public async Task<IActionResult> UpgradeTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }


        [HttpPost("downgrade")]
        public async Task<IActionResult> DowngradeTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }

        [HttpGet("{name}/health-check")]
        public async Task<IActionResult> HealthCheckAsync(string name, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(name);
        }

        [HttpPost("health-status-unhealthy")]
        public async Task<IActionResult> InformHealthStatusUnhealthyAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }
        #endregion



        public record TenantModel
        {
            public string? TenantName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
        }
    }
}