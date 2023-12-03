using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Authorize(Policy = AuthPolicy.SuperAdmin, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public partial class WorkflowController : BaseManagementApiController
    {
        #region Props 
        private readonly ILogger<WorkflowController> _logger;
        private readonly ITenantWorkflow _tenantWorkflowService;
        #endregion

        #region Corts
        public WorkflowController(ILogger<WorkflowController> logger,
                                ITenantWorkflow tenantWorkflowService)
        {
            _logger = logger;
            _tenantWorkflowService = tenantWorkflowService;
        }
        #endregion

        #region Actions   



        [HttpGet()]
        public async Task<IActionResult> GetAllStagesAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _tenantWorkflowService.GetAllStagesAsync(cancellationToken));
        }


        [HttpGet("Duplicates")]
        public async Task<IActionResult> FindDuplicatesStagesAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _tenantWorkflowService.FindDuplicatesStagesAsync(cancellationToken));
        }

        #endregion

    }


}
