using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.Orders.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    [Authorize(Policy = AuthPolicy.Management.Orders, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class OrdersController : BaseManagementApiController
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



        [HttpPut("{id}/plan")]
        public async Task<IActionResult> ChangeOrderPlanAsync([FromRoute] Guid id, ChangeOrderPlanModel model, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _orderService.ChangeOrderPlanAsync(id, model, cancellationToken));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _orderService.GetOrderByIdAsync(id, cancellationToken));
        }

        [HttpGet($"/{PrefixSuperAdminMainApiRoute}/tenants/{{tenantId}}/[controller]")]
        public async Task<IActionResult> GetOrdersListAsync([FromRoute] Guid tenantId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _orderService.GetOrdersListAsync(tenantId, cancellationToken));
        }
        #endregion


    }
}
