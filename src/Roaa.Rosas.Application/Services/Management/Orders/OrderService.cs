using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Orders.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Utilities;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
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
        private readonly int _quantity = 1;
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
                                                  IsMustChangePlan = order.IsMustChangePlan,
                                                  HasToPay = (order.TenantId == null || order.Tenant.LastOrderId == order.Id) &&
                                                                                 (order.OrderStatus == OrderStatus.Initial || order.OrderStatus == OrderStatus.PendingToPay) &&
                                                                                 (order.PaymentStatus == PaymentStatus.Initial || order.PaymentStatus == PaymentStatus.PendingToPay) &&
                                                                                 order.OrderTotal > 0,

                                                  OrderItems = order.OrderItems.Select(orderItem => new OrderItemDto
                                                  {
                                                      Id = orderItem.Id,
                                                      OrderId = orderItem.OrderId,
                                                      CustomPeriodInDays = orderItem.CustomPeriodInDays,
                                                      DisplayName = orderItem.DisplayName,
                                                      EndDate = orderItem.EndDate,
                                                      PlanId = orderItem.PlanId,
                                                      PlanPriceId = orderItem.PlanPriceId,
                                                      ProductId = orderItem.ProductId,
                                                      Quantity = orderItem.Quantity,
                                                      StartDate = orderItem.StartDate,
                                                      SubscriptionId = orderItem.SubscriptionId,
                                                      SystemName = orderItem.SystemName,
                                                      UnitPriceExclTax = orderItem.UnitPriceExclTax,
                                                      UnitPriceInclTax = orderItem.UnitPriceInclTax


                                                  }).ToList()
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
                                                  IsMustChangePlan = order.IsMustChangePlan,
                                                  HasToPay = (order.TenantId == null || order.Tenant.LastOrderId == order.Id) &&
                                                                                 (order.OrderStatus == OrderStatus.Initial || order.OrderStatus == OrderStatus.PendingToPay) &&
                                                                                 (order.PaymentStatus == PaymentStatus.Initial || order.PaymentStatus == PaymentStatus.PendingToPay) &&
                                                                                 order.OrderTotal > 0,
                                                  OrderItems = order.OrderItems.Select(orderItem => new OrderItemDto
                                                  {
                                                      Id = orderItem.Id,
                                                      OrderId = orderItem.OrderId,
                                                      CustomPeriodInDays = orderItem.CustomPeriodInDays,
                                                      DisplayName = orderItem.DisplayName,
                                                      EndDate = orderItem.EndDate,
                                                      PlanId = orderItem.PlanId,
                                                      PlanPriceId = orderItem.PlanPriceId,
                                                      ProductId = orderItem.ProductId,
                                                      Quantity = orderItem.Quantity,
                                                      StartDate = orderItem.StartDate,
                                                      SubscriptionId = orderItem.SubscriptionId,
                                                      SystemName = orderItem.SystemName,
                                                      UnitPriceExclTax = orderItem.UnitPriceExclTax,
                                                      UnitPriceInclTax = orderItem.UnitPriceInclTax


                                                  }).ToList()
                                              })
                                              .ToListAsync(cancellationToken);

            return Result<List<OrderDto>>.Successful(orders);
        }
        public Order BuildOrderEntity(string tenantName, string tenantDisplayName, List<TenantCreationPreparationModel> plansDataList)
        {
            var date = DateTime.UtcNow;

            var isMustChangePlan = plansDataList.Any(x => x.HasTrial) && plansDataList.Any(x => x.Product.TrialType == ProductTrialType.ProductHasTrialPlan);


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
                PriceExclTax = planData.PlanPrice.Price * _quantity,
                PriceInclTax = planData.PlanPrice.Price * _quantity,
                UnitPriceExclTax = planData.PlanPrice.Price,
                UnitPriceInclTax = planData.PlanPrice.Price,
                Quantity = _quantity,
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
                IsMustChangePlan = isMustChangePlan,
            };
        }


        public async Task MarkOrderAsUpgradingFromTrialToRegularSubscriptionAsync(Order order, CancellationToken cancellationToken = default)
        {
            order.OrderIntent = OrderIntent.UpgradingFromTrialToRegularSubscription;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }


        public async Task<Result> ChangeOrderPlanAsync(Guid orderId, ChangeOrderPlanModel model, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                        .Include(x => x.OrderItems)
                                            .Where(x => _identityContextService.IsSuperAdmin() ||
                                                _dbContext.EntityAdminPrivileges
                                                        .Any(a =>
                                                            a.UserId == _identityContextService.UserId &&
                                                            a.EntityId == x.TenantId &&
                                                            a.EntityType == EntityType.Tenant
                                                            )
                                            )
                                              .Where(x => x.Id == orderId)
                                              .SingleOrDefaultAsync(cancellationToken);

            if (order is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (order.OrderStatus == OrderStatus.Complete)
            {
                return Result.Fail(ErrorMessage.CompletedOrderCannotBeProcessed, _identityContextService.Locale);
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                return Result.Fail(ErrorMessage.CanceledOrderCannotBeProcessed, _identityContextService.Locale);
            }

            if (order.PaymentStatus != PaymentStatus.Initial && order.PaymentStatus != PaymentStatus.PendingToPay)
            {
                return Result.Fail(ErrorMessage.OrderCannotBeProcessedWithPaymentStatus, _identityContextService.Locale);
            }

            var tenant = await _dbContext.Tenants
                                        .Where(x => x.Id == order.TenantId)
                                        .Select(x => new { x.SystemName, x.DisplayName })
                                        .SingleOrDefaultAsync();

            if (tenant is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, "Tenant");
            }

            var date = DateTime.UtcNow;



            foreach (var item in order.OrderItems)
            {
                var planPrice = await _dbContext.PlanPrices
                                          .Include(p => p.Plan)
                                          .Include(p => p.Plan.Product)
                                         .Where(x => x.Id == model.PlanPriceId &&
                                                    x.PlanId == model.PlanId &&
                                                    x.Plan.ProductId == item.ProductId)
                                         .SingleOrDefaultAsync();
                if (planPrice is null)
                {
                    return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, "PlanPriceId");
                }


                item.StartDate = date;
                item.EndDate = PlanCycleManager.FromKey(planPrice.PlanCycle).CalculateExpiryDate(date, null);
                item.PlanId = planPrice.PlanId;
                item.PlanPriceId = planPrice.Id;
                item.PriceExclTax = planPrice.Price * _quantity;
                item.PriceInclTax = planPrice.Price * _quantity;
                item.UnitPriceExclTax = planPrice.Price;
                item.UnitPriceInclTax = planPrice.Price;
                item.Quantity = _quantity;
                item.SystemName = $"{planPrice.Plan.Product.SystemName}--{planPrice.Plan.SystemName}--{tenant.SystemName}";
                item.DisplayName = $"[Product: {planPrice.Plan.Product.DisplayName}], [Plan: {planPrice.Plan.DisplayName}], [Tenant: {tenant.DisplayName}]";
            }

            order.OrderSubtotalExclTax = order.OrderItems.Select(x => x.PriceExclTax).Sum();
            order.OrderSubtotalInclTax = order.OrderItems.Select(x => x.PriceInclTax).Sum();
            order.OrderTotal = order.OrderItems.Select(x => x.PriceInclTax).Sum();
            order.OrderIntent = OrderIntent.UpgradingFromTrialToRegularSubscription;
            order.ModifiedByUserId = _identityContextService.GetActorId();
            order.ModificationDate = date;
            order.IsMustChangePlan = false;

            await _dbContext.SaveChangesAsync(cancellationToken);


            return Result.Successful();
        }


        #endregion
    }
}
