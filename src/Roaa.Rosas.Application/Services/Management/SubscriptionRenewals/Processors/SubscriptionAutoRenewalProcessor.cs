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

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.EventHandlers
{
    [SubscriptionRenewalType(SubscriptionRenewalTypeEnum.AutoRenewal)]
    public class SubscriptionAutoRenewalProcessor : SubscriptionRenewalProcessor
    {
        #region Props   
        private readonly ILogger<SubscriptionAutoRenewalProcessor> _logger;
        public override OrderType OrderType { get; set; } = OrderType.SubscriptionAutoRenewal;
        public override PaymentPurpose PaymentPurpose { get; set; } = PaymentPurpose.SubscriptionAutoRenewal;
        #endregion


        #region Corts 
        public SubscriptionAutoRenewalProcessor(IIdentityContextService identityContextService,
                                                          IExternalSystemAPI externalSystemAPI,
                                                          IProductService productService,
                                                          ITenantService tenantService,
                                                          ISubscriptionService subscriptionService,
                                                          IGenericAttributeService genericAttributeService,
                                                          IRosasDbContext dbContext,
                                                          IPublisher publisher,
                                                          ILogger<SubscriptionAutoRenewalProcessor> logger)
         : base(identityContextService, externalSystemAPI, productService, tenantService, subscriptionService, genericAttributeService, dbContext, publisher)
        {
            _logger = logger;
        }
        #endregion

        #region varibles 
        private List<PlanFeatureInfoModel> _subscriptionPlanFeatures;
        #endregion

        public override async Task Handle(SubscriptionRenewalPreparationModel model, CancellationToken cancellationToken)
        {
            await PublishOperationPreparedEventAsync(TenantProcessType.SubscriptionAutoRenewalProcessing, model.Subscription, cancellationToken);

            await UpdateSubscriptionRenewalStatusAsync(model.SubscriptionRenewal, SubscriptionRenewalStatus.Processing, cancellationToken);


            if (!model.SubscriptionRenewal.IsContinuousRenewal && model.SubscriptionRenewal.RenewalsCount <= 0)
            {
                _dbContext.SubscriptionRenewals.Remove(model.SubscriptionRenewal);
            }

            await TryRemovingForcedDowngradeAttributesAsync(model.SubscriptionRenewal.SubscriptionId);

            await _subscriptionService.RenewSubscriptionAsync(model.Subscription, model.SubscriptionRenewal, true, cancellationToken);

            model.SubscriptionRenewal.AddDomainEvent(new SubscriptionRenewedEvent(
                                                     model.Subscription,
                                                     model.SubscriptionRenewal,
                                                     "The Subscription Is Automatically Renewed."));

            if (model.SubscriptionRenewal.RenewalsCount > 0)
            {
                model.SubscriptionRenewal.RenewalsCount -= 1;
            }

            ArgumentNullException.ThrowIfNull(model.Subscription.EndDate);
            model.SubscriptionRenewal.SubscriptionRenewalDate = model.Subscription.EndDate.Value;
            model.SubscriptionRenewal.Status = SubscriptionRenewalStatus.None;


            await _dbContext.SaveChangesAsync(cancellationToken);
        }





        //public override async Task<List<PlanFeatureInfoModel>> FetchSubscriptionPlanFeaturesAsync(Subscription subscription,
        //                                                                                          SubscriptionRenewal autoRenewal,
        //                                                                                          CancellationToken cancellationToken = default)
        //{
        //    if (_subscriptionPlanFeatures is not null) return _subscriptionPlanFeatures;

        //    var subscriptionFeatures = await FetchSubscriptionFeaturesAsync(subscription.Id, cancellationToken);

        //    var subscriptionFeatureCyclesIds = subscriptionFeatures.Select(x => x.SubscriptionFeatureCycleId).ToList();

        //    _subscriptionPlanFeatures = await _dbContext.SubscriptionFeatureCycles.Where(x => subscriptionFeatureCyclesIds.Contains(x.Id))
        //                                                                           .Select(x => new PlanFeatureInfoModel
        //                                                                           {
        //                                                                               PlanFeatureId = x.Id,
        //                                                                               FeatureId = x.FeatureId,
        //                                                                               FeatureUnit = x.FeatureUnit,
        //                                                                               PlanId = autoRenewal.PlanId,
        //                                                                               Limit = x.Limit,
        //                                                                               FeatureDisplayName = x.FeatureDisplayName,
        //                                                                               FeatureType = x.FeatureType,
        //                                                                               FeatureReset = x.FeatureReset,
        //                                                                           }).ToListAsync(cancellationToken);
        //    return _subscriptionPlanFeatures;
        //}

    }
}
