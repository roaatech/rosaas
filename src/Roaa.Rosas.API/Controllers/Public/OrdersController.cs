using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Public
{
    [Route($"{PublicApiRoute}/[controller]")]
    public class OrdersController : BaseRosasPublicApiController
    {
        #region Props 
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderService _orderService;
        #endregion

        #region Corts
        public OrdersController(ILogger<OrdersController> logger,
                                IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }
        #endregion

        #region Actions   



        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdForAnonymousAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _orderService.GetOrderByIdForAnonymousAsync(id, cancellationToken));
        }
        #endregion

    }
}
