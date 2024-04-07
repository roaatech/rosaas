using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Constatns;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Factories;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.RenwalSubscription;

public class RenwalSubscriptionCommandHandler : IRequestHandler<RenwalSubscriptionCommand, Result>
{
    #region Props 
    private readonly ILogger<RenwalSubscriptionCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly ITenantService _tenantService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ISubscriptionRenewalFactory _subscriptionRenewalFactory;
    private readonly SubscriptionRenewalUtilities _utilities;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public RenwalSubscriptionCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionService subscriptionService,
                                                    IPaymentService paymentService,
                                                    IOrderService orderService,
                                                    ITenantService tenantService,
                                                    IRosasDbContext dbContext,
                                                    IGenericAttributeService genericAttributeService,
                                                    ISubscriptionRenewalFactory subscriptionRenewalFactory,
                                                    SubscriptionRenewalUtilities utilities,
                                                    ILogger<RenwalSubscriptionCommandHandler> logger)
    {
        _subscriptionRenewalFactory = subscriptionRenewalFactory;
        _genericAttributeService = genericAttributeService;
        _identityContextService = identityContextService;
        _subscriptionService = subscriptionService;
        _paymentService = paymentService;
        _orderService = orderService;
        _tenantService = tenantService;
        _dbContext = dbContext;
        _utilities = utilities;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(RenwalSubscriptionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var fromDate = DateTime.UtcNow;
            var toDate = fromDate.AddHours(command.RangeInHoursBetweenDates);

            var subscriptionRenewals = await _dbContext.SubscriptionRenewals
                                                     .Include(x => x.Subscription)
                                                     .Where(x => x.SubscriptionRenewalDate >= fromDate &&
                                                                 x.SubscriptionRenewalDate <= toDate)
                                                     .ToListAsync(cancellationToken);

            foreach (var subscriptionRenewal in subscriptionRenewals)
            {
                if (!EnsureSubscriptionHasNotForcedDowngrade(subscriptionRenewal, subscriptionRenewals))
                    continue;

                ArgumentNullException.ThrowIfNull(subscriptionRenewal.Subscription);

                if (!_utilities.EnsureAllowedSubscriptionRenewalStatus(subscriptionRenewal.Status))
                {
                    throw new NullReferenceException($"Cannot renew({subscriptionRenewal.Type}) the subscription in {subscriptionRenewal.Status} status.");
                }

                var subscriptionRenewalProcessor = _subscriptionRenewalFactory.InstantiateProcessor(subscriptionRenewal.Type);

                if (!_utilities.EnsureIsForcedDowngrade(subscriptionRenewal))
                {
                    await UpdateSubscriptionRenewalStatusAsync(subscriptionRenewal, SubscriptionRenewalStatus.PendingPayment, cancellationToken);

                    var linkedCard = await _dbContext.LinkedCards.Where(x => x.EntityId == subscriptionRenewal.Id &&
                                                                             x.EntityType == Common.Enums.EntityType.SubscriptionRenewal)
                                                                 .SingleOrDefaultAsync(cancellationToken);
                    ArgumentNullException.ThrowIfNull(linkedCard);

                    var orderId = await _genericAttributeService.GetAttributeAsync<SubscriptionRenewal, Guid?>(
                                                           subscriptionRenewal.Id,
                                                           Consts.GenericAttributeKey.OrderOfSubscriptionRenewal,
                                                           null,
                                                           cancellationToken);
                    Order? order;

                    if (orderId is null)
                    {
                        order = await GenerateOrderAsync(subscriptionRenewal.Subscription, subscriptionRenewal, linkedCard, subscriptionRenewalProcessor.OrderType, cancellationToken);
                        await _genericAttributeService.SaveAttributeAsync<SubscriptionRenewal, Guid?>(subscriptionRenewal.Id,
                                                                                                Consts.GenericAttributeKey.OrderOfSubscriptionRenewal,
                                                                                                 order.Id,
                                                                                                 cancellationToken);
                    }
                    else
                    {
                        order = await _dbContext.Orders
                                               .Include(x => x.OrderItems)
                                               .Where(x => x.Id == orderId)
                                               .SingleOrDefaultAsync(cancellationToken);
                        ArgumentNullException.ThrowIfNull(order);
                    }

                    var paymentResult = await _paymentService.PayAsync(order,
                                                     linkedCard.ReferenceId,
                                                     subscriptionRenewalProcessor.PaymentPurpose,
                                                     subscriptionRenewal.CreatedByUserId,
                                                     subscriptionRenewal.CreatedByUserType,
                                                     cancellationToken);
                    if (!paymentResult.Success)
                    {
                        await UpdateSubscriptionRenewalStatusAsync(subscriptionRenewal, SubscriptionRenewalStatus.FailedPayment, cancellationToken);
                        continue;
                    }
                }

                await UpdateSubscriptionRenewalStatusAsync(subscriptionRenewal, SubscriptionRenewalStatus.preparing, cancellationToken);

                var preparationModel = new SubscriptionRenewalPreparationModel(subscriptionRenewal.Subscription, subscriptionRenewal);

                await subscriptionRenewalProcessor.Handle(preparationModel, cancellationToken);

                await _subscriptionService.ActivateSubscriptionAsync(subscriptionRenewal.Subscription, cancellationToken);
            }
            return Result.Successful();
        }

        catch (Exception ex)
        {
            var errorMsg = $"An error occurred while executing the {nameof(RenwalSubscriptionCommandHandler)}!";
            _logger.LogError(ex, errorMsg);
            return Result.Fail(errorMsg);
        }
    }

    #endregion
    public async Task<Order> GenerateOrderAsync(Subscription subscription,
                                                SubscriptionRenewal subscriptionRenewal,
                                                LinkedCard linkedCard,
                                                OrderType orderType,
                                                CancellationToken cancellationToken = default)
    {
        var tenantResult = await _dbContext.Tenants
                                       .Where(x => x.Id == subscription.TenantId)
                                       .Select(x => new { x.SystemName, x.DisplayName })
                                       .SingleOrDefaultAsync(cancellationToken);
        ArgumentNullException.ThrowIfNull(tenantResult);

        var productResult = await _dbContext.Products
                                      .Where(x => x.Id == subscription.ProductId)
                                      .Select(x => new { x.DisplayName, x.ClientId })
                                      .SingleOrDefaultAsync(cancellationToken);
        ArgumentNullException.ThrowIfNull(productResult);


        var planPrice = await _dbContext.PlanPrices
                                    .Where(x => x.Id == subscriptionRenewal.PlanPriceId)
                                    .Include(x => x.Plan)
                                    .SingleOrDefaultAsync(cancellationToken);
        ArgumentNullException.ThrowIfNull(planPrice);

        var planFeatureInfos = await _subscriptionService.FetchSubscriptionPlanFeaturesAsync(subscription, subscriptionRenewal, cancellationToken);

        var orderItem = _orderService.BuildOrderItemEntity(tenantResult.SystemName,
                                           tenantDisplayName: tenantResult.DisplayName,
                                           clientId: productResult.ClientId,
                                           sequenceNum: 1,
                                           productId: subscription.ProductId,
                                           productDisplayName: productResult.DisplayName,
                                           planId: subscriptionRenewal.PlanId,
                                           planDisplayName: subscriptionRenewal.PlanDisplayName,
                                           cycle: subscriptionRenewal.PlanCycle,
                                           tenancyType: TenancyType.Planed,
                                           planPriceId: subscriptionRenewal.PlanPriceId,
                                           price: subscriptionRenewal.Price,
                                           trialPeriodInDays: 0,
                                           customPeriodInDays: null,
                                           date: DateTime.UtcNow,
                                           planFeatures: planFeatureInfos);

        orderItem.SubscriptionId = subscription.Id;

        var order = _orderService.BuildOrderEntity(
                                  orderItems: new List<OrderItem> { orderItem },
                                  date: DateTime.UtcNow,
                                  orderType: orderType,
                                  paymentMethodType: PaymentMethodType.Card,
                                  paymentPlatform: linkedCard.PaymentPlatform,
                                  userId: _identityContextService.GetActorId(),
                                  userType: _identityContextService.GetUserType());


        order.CreatedByUserId = subscriptionRenewal.CreatedByUserId;
        order.ModifiedByUserId = subscriptionRenewal.ModifiedByUserId;
        order.CreatedByUserType = subscriptionRenewal.CreatedByUserType;

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return order;
    }


    private async Task UpdateSubscriptionRenewalStatusAsync(SubscriptionRenewal subscriptionRenewal, SubscriptionRenewalStatus renewalStatus, CancellationToken cancellationToken = default)
    {
        subscriptionRenewal.Status = renewalStatus;
        subscriptionRenewal.ModificationDate = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    private bool EnsureSubscriptionHasNotForcedDowngrade(SubscriptionRenewal subscriptionRenewal, List<SubscriptionRenewal> subscriptionRenewals)
    {
        if (!subscriptionRenewal.IsForced &&
                   subscriptionRenewals.Where(x => x.Id != subscriptionRenewal.Id &&
                                                   x.SubscriptionId == subscriptionRenewal.SubscriptionId &&
                                                   x.IsForced).Any())
        {
            return false;
        }
        return true;
    }
}

