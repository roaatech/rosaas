using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Orders
{
    public class OrderService : IOrderService
    {
        #region Props 
        private readonly ILogger<OrderService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public OrderService(
            ILogger<OrderService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services  
        public async Task<Result<OrderDto>> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                              .AsNoTracking()
                                            .Where(x => _identityContextService.IsSuperAdmin() ||
                                                _dbContext.EntityAdminPrivileges
                                                        .Any(a =>
                                                            a.UserId == _identityContextService.UserId &&
                                                            a.EntityId == x.TenantId &&
                                                            a.EntityType == EntityType.Tenant
                                                            )
                                            )
                                              .Where(x => x.Id == orderId)
                                              .Select(order => new OrderDto
                                              {
                                                  Id = order.Id,
                                                  TenantId = order.TenantId,
                                                  OrderStatus = order.OrderStatus,
                                                  OrderSubtotalExclTax = order.OrderSubtotalExclTax,
                                                  OrderSubtotalInclTax = order.OrderSubtotalInclTax,
                                                  OrderTotal = order.OrderTotal,
                                                  PaidDate = order.PaidDate,
                                                  PaymentMethodType = order.PaymentMethodType,
                                                  PaymentStatus = order.PaymentStatus,
                                                  UserCurrencyType = order.UserCurrencyType,
                                                  UserCurrencyCode = order.UserCurrencyCode,
                                                  CreatedDate = order.CreationDate,
                                                  EditedDate = order.ModificationDate,
                                              })
                                              .SingleOrDefaultAsync(cancellationToken);

            return Result<OrderDto>.Successful(order);
        }


        #endregion
    }
}
