using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Application.Services.Management.Tenants.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.ExternalSystem
{

    public class TenantController : BaseExternalSystemMainApiController
    {
        #region Props 
        private readonly ILogger<TenantController> _logger;
        private readonly ITenantService _tenantService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public TenantController(ILogger<TenantController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                                ITenantService tenantService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
        }
        #endregion


        #region Actions   


        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetTenantStatusByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _tenantService.GetTenantStatusByIdAsync(new TenantStatusModel(id, _identityContextService.GetProductId()), cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantByExternalSystemModel model, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _tenantService.CreateTenantAsync(new CreateTenantModel(model, _identityContextService.GetProductId()), _identityContextService.GetProductId(), cancellationToken));
        }

        #endregion 
    }
}
