using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.EventHandlers
{
    [SubscriptionRenewalType(SubscriptionRenewalTypeEnum.ForcedDowngrade)]
    public class SubscriptionForcedDowngradeProcessor : SubscriptionDowngradeProcessor
    {
        #region Props   
        private readonly ILogger<SubscriptionDowngradeProcessor> _logger;

        public override OrderType OrderType { get; set; } = OrderType.DowngradeSubscription;
        public override PaymentPurpose PaymentPurpose { get; set; } = PaymentPurpose.DowngradeSubscription;
        #endregion


        #region Corts 
        public SubscriptionForcedDowngradeProcessor(IIdentityContextService identityContextService,
                                                         IExternalSystemAPI externalSystemAPI,
                                                         IProductService productService,
                                                         ITenantService tenantService,
                                                         ISubscriptionService subscriptionService,
                                                         IGenericAttributeService genericAttributeService,
                                                         IRosasDbContext dbContext,
                                                         IPublisher publisher,
                                                         ILogger<SubscriptionForcedDowngradeProcessor> logger)
        : base(identityContextService, externalSystemAPI, productService, tenantService, subscriptionService, genericAttributeService, dbContext, publisher, logger)
        {
            _logger = logger;
        }
        #endregion


        public override async Task Handle(SubscriptionRenewalPreparationModel model, CancellationToken cancellationToken)
        {
            await base.Handle(model, cancellationToken);
        }
        public override async Task HandlesubscriptionRenewalAsync(Subscription subscription, SubscriptionRenewal subscriptionRenewal, CancellationToken cancellationToken)
        {
            if (!await TryRemovingForcedDowngradeAttributesAsync(subscription.Id))
            {
                _dbContext.SubscriptionRenewals.Remove(subscriptionRenewal);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }


    }
}