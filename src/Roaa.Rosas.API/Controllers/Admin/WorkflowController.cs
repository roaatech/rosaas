using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    public partial class WorkflowController : BaseSuperAdminMainApiController
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
