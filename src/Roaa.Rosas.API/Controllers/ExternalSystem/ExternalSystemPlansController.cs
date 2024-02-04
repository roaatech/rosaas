using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Services.Management.Plans;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.ExternalSystem
{
    [Route("api/management/apps/v1/plans")]
    public class ExternalSystemPlansController : BaseExternalSystemApiController
    {
        #region Props  
        private readonly IPlanService _planService;
        private readonly IIdentityContextService _identityContextService;

        #endregion

        #region Corts
        public ExternalSystemPlansController(IPlanService planService,
                                 IIdentityContextService identityContextService
                                 )
        {
            _identityContextService = identityContextService;
            _planService = planService;
        }
        #endregion


        #region Actions  



        [HttpGet()]
        public async Task<IActionResult> GetPlansListOfExternalSystemByProductIdAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _planService.GetPlansListOfExternalSystemByProductIdAsync(_identityContextService.GetProductId(), cancellationToken));
        }




        #endregion
    }
}
