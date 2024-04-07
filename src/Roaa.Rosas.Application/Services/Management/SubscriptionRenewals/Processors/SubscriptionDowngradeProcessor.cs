using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Processors.Abstraction;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.EventHandlers
{
    [SubscriptionRenewalType(SubscriptionRenewalTypeEnum.Downgrade)]
    public class SubscriptionDowngradeProcessor : SubscriptionRenewalProcessor
    {
        #region Props   
        private readonly ILogger<SubscriptionDowngradeProcessor> _logger;

        public override OrderType OrderType { get; set; } = OrderType.DowngradeSubscription;
        public override PaymentPurpose PaymentPurpose { get; set; } = PaymentPurpose.DowngradeSubscription;
        #endregion 


        #region Corts 
        public SubscriptionDowngradeProcessor(IIdentityContextService identityContextService,
                                                          IExternalSystemAPI externalSystemAPI,
                                                          IProductService productService,
                                                          ITenantService tenantService,
                                                          ISubscriptionService subscriptionService,
                                                          IRosasDbContext dbContext,
                                                          IPublisher publisher,
                                                          ILogger<SubscriptionDowngradeProcessor> logger)
         : base(identityContextService, externalSystemAPI, productService, tenantService, subscriptionService, dbContext, publisher)
        {
            _logger = logger;
        }
        #endregion



        public override async Task Handle(SubscriptionRenewalPreparationModel model, CancellationToken cancellationToken)
        {
            await PublishOperationPreparedEventAsync(TenantProcessType.SubscriptionDowngradePrepared, model.Subscription, cancellationToken);

            // External System's url preparation
            Expression<Func<Product, ExternalSystemApiModel>> selector = x => new ExternalSystemApiModel(x.ApiKey, x.SubscriptionDowngradeUrl, x.ApplySubscriptionDowngradeByExternalSystemAction);

            var requestModel = await BuildRequestModelAsync<DowngradeTenantModel>(model.Subscription.ProductId, model.Subscription.TenantId, selector, cancellationToken);
            requestModel.Data = new()
            {
                TenantName = await GetTenantSystemNameAsync(model.Subscription.TenantId, cancellationToken),
            };

            // External System calling to downgrade the tenant resorces 
            var callingResult = await _externalSystemAPI.DowngradeTenantAsync(requestModel, cancellationToken);

            await UpdateSubscriptionRenewalStatusAsync(model.SubscriptionRenewal, SubscriptionRenewalStatus.PendingExternalSystem, cancellationToken);

            await PublishOperationPreparedEventAsync(TenantProcessType.SubscriptionDowngradeBeingApplied, model.Subscription, cancellationToken);

            if (!ApplySubscriptionRenewalByExternalSystemAction)
            {
                await ApplyDowngradeToSubscriptionAsync(model.Subscription, model.SubscriptionRenewal, cancellationToken);
            }
        }

        public async Task ApplyDowngradeToSubscriptionAsync(Subscription subscription, SubscriptionRenewal subscriptionRenewal, CancellationToken cancellationToken)
        {
            await UpdateSubscriptionRenewalStatusAsync(subscriptionRenewal, SubscriptionRenewalStatus.Processing, cancellationToken);

            await _subscriptionService.RenewSubscriptionAsync(subscription, subscriptionRenewal, false, cancellationToken);

            if (!subscriptionRenewal.IsContinuousRenewal && subscriptionRenewal.RenewalsCount <= 0)
            {
                _dbContext.SubscriptionRenewals.Remove(subscriptionRenewal);
            }
            else
            {
                ArgumentNullException.ThrowIfNull(subscription.EndDate);
                subscriptionRenewal.SubscriptionRenewalDate = subscription.EndDate.Value;
                subscriptionRenewal.Status = SubscriptionRenewalStatus.None;
                subscriptionRenewal.ModificationDate = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}


/*
    var subscriptionRenewalHistory = new SubscriptionRenewalHistory
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscriptionRenewal.SubscriptionId,
                PlanId = subscriptionRenewal.PlanId,
                PlanPriceId = subscriptionRenewal.PlanPriceId,
                Type = subscriptionRenewal.Type,
                PlanCycle = subscriptionRenewal.PlanCycle,
                Price = subscriptionRenewal.Price,
                RenewalEnabledByUserId = subscriptionRenewal.ModifiedByUserId,
                RenewalEnabledDate = subscriptionRenewal.ModificationDate,
                Comment = subscriptionRenewal.Comment,
                RenewalDate = DateTime.UtcNow,
            };


            _dbContext.SubscriptionRenewalHistories.Add(subscriptionRenewalHistory);
 */