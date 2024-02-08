using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Specifications;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Public
{
    public class SpecificationsController : BaseRosasPublicApiController
    {
        #region Props 
        private readonly ILogger<SpecificationsController> _logger;
        private readonly ISpecificationService _specificationService;
        private readonly ISender _mediator;
        #endregion

        #region Corts
        public SpecificationsController(ILogger<SpecificationsController> logger,
                                           ISpecificationService specificationService,
                                            ISender mediator)
        {
            _logger = logger;
            _specificationService = specificationService;
            _mediator = mediator;
        }
        #endregion

        #region Actions    
        [HttpGet("Product/{name}/[controller]")]
        public async Task<IActionResult> GetSpecificationPublishedListAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return ListResult(await _specificationService.GetSpecificationsListByProductNamesync(name, cancellationToken));
        }
        #endregion


    }
}
