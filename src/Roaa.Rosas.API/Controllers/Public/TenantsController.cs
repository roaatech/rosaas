using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Public
{
    public class TenantsController : BaseRosasPublicApiController
    {
        #region Props 
        private readonly ILogger<SpecificationsController> _logger;
        private readonly ISender _mediator;
        #endregion

        #region Corts
        public TenantsController(ILogger<SpecificationsController> logger,
                                IWebHostEnvironment environment,
                                ISender mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        #endregion

        #region Actions    

        [HttpPost("[controller]")]
        public async Task<IActionResult> CreateTenantAsync([FromBody] TenantCreationRequestCommand command, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(command, cancellationToken));
        }
        #endregion


    }
}
