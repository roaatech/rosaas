using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Processors.Abstraction;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.EventHandlers
{
    [SubscriptionRenewalType(SubscriptionRenewalTypeEnum.Upgrade)]
    public class SubscriptionUpgradingProcessor : SubscriptionRenewalProcessor
    {
        #region Props   
        private readonly ILogger<SubscriptionUpgradingProcessor> _logger;

        public override OrderType OrderType { get; set; } = OrderType.UpgradeSubscription;
        public override PaymentPurpose PaymentPurpose { get; set; } = PaymentPurpose.UpgradeSubscription;
        #endregion


        #region Corts 
        public SubscriptionUpgradingProcessor(IIdentityContextService identityContextService,
                                                        IExternalSystemAPI externalSystemAPI,
                                                        IProductService productService,
                                                        ITenantService tenantService,
                                                        ISubscriptionService subscriptionService,
                                                        IGenericAttributeService genericAttributeService,
                                                        IRosasDbContext dbContext,
                                                        IPublisher publisher,
                                                        ILogger<SubscriptionUpgradingProcessor> logger)
       : base(identityContextService, externalSystemAPI, productService, tenantService, subscriptionService, genericAttributeService, dbContext, publisher)
        {
            _logger = logger;
        }
        #endregion 


        public override async Task Handle(SubscriptionRenewalPreparationModel model, CancellationToken cancellationToken)
        {
            // External System's url preparation
            Expression<Func<Product, ExternalSystemApiModel>> selector = x => new ExternalSystemApiModel(x.ApiKey, x.SubscriptionUpgradeUrl, x.ApplySubscriptionUpgradeByExternalSystemAction);

            var requestModel = await BuildRequestModelAsync<UpgradeTenantModel>(model.Subscription.ProductId, model.Subscription.TenantId, selector, cancellationToken);
            requestModel.Data = new()
            {
                TenantName = await GetTenantSystemNameAsync(model.Subscription.TenantId, cancellationToken),
            };

            // External System calling to downgrade the tenant resorces 
            var callingResult = await _externalSystemAPI.UpgradeTenantAsync(requestModel, cancellationToken);

            await UpdateSubscriptionRenewalStatusAsync(model.SubscriptionRenewal, SubscriptionRenewalStatus.PendingExternalSystem, cancellationToken);

            await PublishOperationPreparedEventAsync(TenantProcessType.SubscriptionUpgradeBeingApplied, model.Subscription, cancellationToken);

            if (!ApplySubscriptionRenewalByExternalSystemAction)
            {
                await ApplyUpgradeToSubscriptionAsync(model.Subscription, model.SubscriptionRenewal, cancellationToken);
            }
        }

        public async Task ApplyUpgradeToSubscriptionAsync(Subscription subscription, SubscriptionRenewal subscriptionRenewal, CancellationToken cancellationToken)
        {
            await UpdateSubscriptionRenewalStatusAsync(subscriptionRenewal, SubscriptionRenewalStatus.Processing, cancellationToken);

            await _subscriptionService.RenewSubscriptionAsync(subscription, subscriptionRenewal, false, cancellationToken);

            await TryRemovingForcedDowngradeAttributesAsync(subscription.Id);

            if (!subscriptionRenewal.IsContinuousRenewal && subscriptionRenewal.RenewalsCount <= 0)
            {
                _dbContext.SubscriptionRenewals.Remove(subscriptionRenewal);
            }
            else
            {
                ArgumentNullException.ThrowIfNull(subscription.EndDate);
                subscriptionRenewal.SubscriptionRenewalDate = subscription.EndDate.Value;
                subscriptionRenewal.Status = SubscriptionRenewalStatus.None;
                subscriptionRenewal.Type = SubscriptionRenewalTypeEnum.AutoRenewal;
                subscriptionRenewal.ModificationDate = DateTime.UtcNow;
            }

            subscriptionRenewal.AddDomainEvent(new SubscriptionHasBeenUpgradedEvent(subscription, subscriptionRenewal));

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
