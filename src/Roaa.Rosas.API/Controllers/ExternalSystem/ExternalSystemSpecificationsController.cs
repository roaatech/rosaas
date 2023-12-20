using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Services.Management.Specifications;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.ExternalSystem
{
    [Route("api/management/apps/v1/specifications")]
    public class ExternalSystemSpecificationsController : BaseExternalSystemApiController
    {
        #region Props  
        private readonly ISpecificationService _specificationService;
        private readonly IIdentityContextService _identityContextService;

        #endregion

        #region Corts
        public ExternalSystemSpecificationsController(ISpecificationService specificationService,
                                 IIdentityContextService identityContextService
                                 )
        {
            _identityContextService = identityContextService;
            _specificationService = specificationService;
        }
        #endregion


        #region Actions  



        [HttpGet()]
        public async Task<IActionResult> GetSpecificationsListOfExternalSystemByProductIdAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _specificationService.GetSpecificationsListOfExternalSystemByProductIdAsync(_identityContextService.GetProductId(), cancellationToken));
        }




        #endregion
    }
}
