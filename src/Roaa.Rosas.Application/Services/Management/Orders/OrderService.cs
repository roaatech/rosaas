using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Orders.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Utilities;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

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
                                                  OrderId = order.Id,
                                                  TenantId = order.TenantId,
                                                  OrderNumber = order.OrderNumber,
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



        public async Task<Result<List<OrderDto>>> GetOrdersListAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            var orders = await _dbContext.Orders
                                              .AsNoTracking()
                                            .Where(x => _identityContextService.IsSuperAdmin() ||
                                                _dbContext.EntityAdminPrivileges
                                                        .Any(a =>
                                                            a.UserId == _identityContextService.UserId &&
                                                            a.EntityId == x.TenantId &&
                                                            a.EntityType == EntityType.Tenant
                                                            )
                                            )
                                              .Where(x => x.TenantId == tenantId)
                                              .Select(order => new OrderDto
                                              {
                                                  Id = order.Id,
                                                  OrderId = order.Id,
                                                  TenantId = order.TenantId,
                                                  OrderNumber = order.OrderNumber,
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
                                                  HasToPay = order.Tenant.LastOrderId == order.Id &&
                                                                                 (order.OrderStatus == OrderStatus.Initial || order.OrderStatus == OrderStatus.PendingToPay) &&
                                                                                 (order.PaymentStatus == PaymentStatus.Initial || order.PaymentStatus == PaymentStatus.PendingToPay),
                                              })
                                              .ToListAsync(cancellationToken);

            return Result<List<OrderDto>>.Successful(orders);
        }
        public Order BuildOrderEntity(string tenantName, string tenantDisplayName, List<TenantCreationPreparationModel> plansDataList)
        {
            var quantity = 1;
            var date = DateTime.UtcNow;
            var orderItems = plansDataList.Select(planData => new OrderItem()
            {
                Id = Guid.NewGuid(),
                StartDate = date,
                EndDate = PlanCycleManager.FromKey(planData.PlanPrice.PlanCycle).CalculateExpiryDate(date, planData.PlanPrice.CustomPeriodInDays, null, planData.Plan.TenancyType),
                ClientId = planData.Product.ClientId,
                ProductId = planData.Product.Id,
                SubscriptionId = planData.GeneratedSubscriptionId,
                PlanId = planData.Plan.Id,
                PlanPriceId = planData.PlanPrice.Id,
                CustomPeriodInDays = planData.PlanPrice.CustomPeriodInDays,
                PriceExclTax = planData.PlanPrice.Price * quantity,
                PriceInclTax = planData.PlanPrice.Price * quantity,
                UnitPriceExclTax = planData.PlanPrice.Price,
                UnitPriceInclTax = planData.PlanPrice.Price,
                Quantity = quantity,
                SystemName = $"{planData.Product.SystemName}--{planData.Plan.SystemName}--{tenantName}",
                DisplayName = $"[Product: {planData.Product.DisplayName}], [Plan: {planData.Plan.DisplayName}], [Tenant: {tenantDisplayName}]",
                Specifications = planData.Features.Select(x => new OrderItemSpecification
                {
                    PurchasedEntityId = x.FeatureId,
                    PurchasedEntityType = Common.Enums.EntityType.Feature,
                    SystemName = $"{x.FeatureName}-" +
                                    $"{(x.Limit.HasValue ? x.Limit : string.Empty)}-" +
                                    $"{(x.FeatureUnit.HasValue ? x.FeatureUnit.ToString() : string.Empty)}-" +
                                    $"{(x.FeatureReset != FeatureReset.NonResettable ? x.FeatureReset.ToString() : string.Empty)}"
                                    .Replace("---", "-")
                                    .Replace("--", "-")
                                    .TrimEnd('-'),
                }).ToList()
            }).ToList();


            return new Order()
            {
                Id = Guid.NewGuid(),
                TenantId = null,
                OrderStatus = OrderStatus.Initial,
                PaymentStatus = PaymentStatus.Initial,
                CurrencyRate = 1,
                UserCurrencyType = CurrencyCode.USD,
                UserCurrencyCode = CurrencyCode.USD.ToString(),
                PaymentMethodType = null,
                CreatedByUserType = _identityContextService.GetUserType(),
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreationDate = date,
                ModificationDate = date,
                OrderSubtotalExclTax = orderItems.Select(x => x.PriceExclTax).Sum(),
                OrderSubtotalInclTax = orderItems.Select(x => x.PriceInclTax).Sum(),
                OrderTotal = orderItems.Select(x => x.PriceInclTax).Sum(),
                OrderItems = orderItems,
                OrderIntent = OrderIntent.TenantCreation,
            };
        }


        public async Task MarkOrderAsUpgradingFromTrialToRegularSubscriptionAsync(Order order, CancellationToken cancellationToken = default)
        {
            order.OrderIntent = OrderIntent.UpgradingFromTrialToRegularSubscription;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }




        #endregion
    }
}
