using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Orders.Models;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Trials;
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
using Roaa.Rosas.Domain.Models.Payment;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Orders
{
    public class OrderService : IOrderService
    {
        #region Props 
        private readonly ILogger<OrderService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly ITrialProcessingService _trialProcessingService;
        private readonly IIdentityContextService _identityContextService;
        private readonly int _quantity = 1;
        #endregion


        #region Corts
        public OrderService(
            ILogger<OrderService> logger,
            IRosasDbContext dbContext,
             ITrialProcessingService trialProcessingService,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _trialProcessingService = trialProcessingService;
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
                                              .Select(GetOrderDtoSelector())
                                              .SingleOrDefaultAsync(cancellationToken);

            return Result<OrderDto>.Successful(order);
        }

        public async Task<Result<OrderDto>> GetOrderByIdForAnonymousAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var allowedOrderStatus = new List<OrderStatus> { OrderStatus.PendingToPay, OrderStatus.Initial };
            var allowedPaymentStatus = new List<PaymentStatus> { PaymentStatus.None };
            var order = await _dbContext.Orders
                                              .AsNoTracking()
                                              .Where(x => x.Id == orderId &&
                                                          allowedOrderStatus.Contains(x.OrderStatus) &&
                                                          (x.PaymentStatus == null || allowedPaymentStatus.Contains(x.PaymentStatus.Value)))
                                              .Select(GetOrderDtoSelector())
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
                                                            ))
                                              .Where(x => x.TenantId == tenantId)
                                              .Select(GetOrderDtoSelector())
                                              .ToListAsync(cancellationToken);

            return Result<List<OrderDto>>.Successful(orders);
        }

        public async Task<Result<List<OrderDto>>> GetOrdersListAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _dbContext.Orders
                                        .AsNoTracking()
                                        .Where(x => _dbContext.EntityAdminPrivileges
                                                .Any(a =>
                                                    a.UserId == _identityContextService.UserId &&
                                                    a.EntityId == x.TenantId &&
                                                    a.EntityType == EntityType.Tenant
                                                    ))
                                        .Select(GetOrderDtoSelector())
                                        .ToListAsync(cancellationToken);

            return Result<List<OrderDto>>.Successful(orders);
        }

        public async Task<Result<List<OrderDto>>> GetOrdersListByPaymentStatusAsync(PaymentStatus paymentStatus, CancellationToken cancellationToken = default)
        {
            var orders = await _dbContext.Orders
                                        .AsNoTracking()
                                        .Where(x => _dbContext.EntityAdminPrivileges
                                                .Any(a =>
                                                    a.UserId == _identityContextService.UserId &&
                                                    a.EntityId == x.TenantId &&
                                                    a.EntityType == EntityType.Tenant
                                                    ))
                                        .Where(x => x.PaymentStatus == paymentStatus)
                                        .Select(GetOrderDtoSelector())
                                        .ToListAsync(cancellationToken);

            return Result<List<OrderDto>>.Successful(orders);
        }

        public async Task<List<KeyValuePair<Guid, PaymentMethodCardDto>>> GetPaymentMethodCardsListAsync(List<Guid?> subscriptions, CancellationToken cancellationToken = default)
        {
            return await _dbContext.OrderItems
                                    .Where(x => x.SubscriptionId != null &&
                                                subscriptions.Contains(x.SubscriptionId) &&
                                                x.Order.PaymentMethod != null)
                                    .Select(x => new KeyValuePair<Guid, PaymentMethodCardDto>(
                                        x.SubscriptionId.Value,
                                        new PaymentMethodCardDto
                                        {
                                            ReferenceId = x.Order.PaymentMethod.Card.ReferenceId,
                                            Brand = x.Order.PaymentMethod.Card.Brand,
                                            ExpirationMonth = x.Order.PaymentMethod.Card.ExpirationMonth,
                                            ExpirationYear = x.Order.PaymentMethod.Card.ExpirationYear,
                                            CardholderName = x.Order.PaymentMethod.Card.CardholderName,
                                            Last4Digits = x.Order.PaymentMethod.Card.Last4Digits,
                                        }))
                                    .ToListAsync(cancellationToken);
        }

        public Expression<Func<Order, OrderDto>> GetOrderDtoSelector()
        {
            return order => new OrderDto
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
                PaymentPlatform = order.PaymentPlatform,
                PaymentStatus = order.PaymentStatus,
                UserCurrencyType = order.UserCurrencyType,
                UserCurrencyCode = order.UserCurrencyCode,
                CreatedDate = order.CreationDate,
                EditedDate = order.ModificationDate,
                IsMustChangePlan = order.IsMustChangePlan,
                PaymentMethodCard = order.PaymentMethod == null ? null : new PaymentMethodCardDto
                {
                    ReferenceId = order.PaymentMethod.Card.ReferenceId,
                    Brand = order.PaymentMethod.Card.Brand,
                    ExpirationMonth = order.PaymentMethod.Card.ExpirationMonth,
                    ExpirationYear = order.PaymentMethod.Card.ExpirationYear,
                    CardholderName = order.PaymentMethod.Card.CardholderName,
                    Last4Digits = order.PaymentMethod.Card.Last4Digits,
                },
                HasToPay = (order.TenantId == null || order.Tenant.LastOrderId == order.Id) &&
                                                                                  (order.OrderStatus == OrderStatus.Initial || order.OrderStatus == OrderStatus.PendingToPay) &&
                                                                                  (order.PaymentStatus == PaymentStatus.None) &&
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
                    UnitPriceInclTax = orderItem.UnitPriceInclTax,
                    TrialPeriodInDays = orderItem.TrialPeriodInDays,


                }).ToList()
            };
        }

        public Order BuildOrderEntity(string tenantName, string tenantDisplayName, List<SubscriptionPreparationModel> plansDataList)
        {
            var date = DateTime.UtcNow;

            //  var isMustChangePlan = plansDataList.Any(x => x.HasTrial) && plansDataList.Any(x => x.Product.TrialType == ProductTrialType.ProductHasTrialPlan);



            var orderItems = plansDataList.Select(item =>
            {
                var orderItem = new OrderItem()
                {
                    Id = Guid.NewGuid(),
                    StartDate = date,
                    EndDate = PlanCycleManager.FromKey(item.PlanPrice.PlanCycle).CalculateExpiryDate(date, item.PlanPrice.CustomPeriodInDays, null, item.Plan.TenancyType),
                    ClientId = item.Product.ClientId,
                    ProductId = item.Product.Id,
                    SequenceNum = item.SequenceNum,
                    // SubscriptionId = planData.GeneratedSubscriptionId,
                    PlanId = item.Plan.Id,
                    PlanPriceId = item.PlanPrice.Id,
                    CustomPeriodInDays = item.PlanPrice.CustomPeriodInDays,
                    PriceExclTax = item.PlanPrice.Price * _quantity,
                    PriceInclTax = item.PlanPrice.Price * _quantity,
                    UnitPriceExclTax = item.PlanPrice.Price,
                    UnitPriceInclTax = item.PlanPrice.Price,
                    Quantity = _quantity,
                    SystemName = $"{tenantName}",
                    DisplayName = $"[Product: {item.Product.DisplayName}], [Plan: {item.Plan.DisplayName}], [Tenant: {tenantDisplayName}]",
                    TrialPeriodInDays = _trialProcessingService.FeatchTrialPeriodInDays(item) ?? 0,
                    Specifications = item.Features.Select(x => new OrderItemSpecification
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
                };
                return orderItem;
            }).ToList();


            return new Order()
            {
                Id = Guid.NewGuid(),
                TenantId = null,
                OrderStatus = OrderStatus.Initial,
                PaymentStatus = PaymentStatus.None,
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
                // IsMustChangePlan = isMustChangePlan,
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

            if (order.PaymentStatus != PaymentStatus.None)
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
                item.SystemName = $"{tenant.SystemName}";
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
