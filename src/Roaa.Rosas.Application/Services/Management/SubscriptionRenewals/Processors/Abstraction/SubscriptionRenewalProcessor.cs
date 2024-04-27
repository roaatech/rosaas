using MediatR;
using Roaa.Rosas.Application.Constatns;
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
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Processors.Abstraction
{
    public abstract class SubscriptionRenewalProcessor
    {
        #region Props  
        public readonly IIdentityContextService _identityContextService;
        public readonly IExternalSystemAPI _externalSystemAPI;
        public readonly IProductService _productService;
        public readonly ITenantService _tenantService;
        public readonly ISubscriptionService _subscriptionService;
        public readonly IRosasDbContext _dbContext;
        public readonly IPublisher _publisher;
        private readonly IGenericAttributeService _genericAttributeService;
        protected bool ApplySubscriptionRenewalByExternalSystemAction { get; private set; }
        #endregion


        #region Corts 
        public SubscriptionRenewalProcessor(IIdentityContextService identityContextService,
                                                            IExternalSystemAPI externalSystemAPI,
                                                            IProductService productService,
                                                            ITenantService tenantService,
                                                            ISubscriptionService subscriptionService,
                                                            IGenericAttributeService genericAttributeService,
                                                            IRosasDbContext dbContext,
                                                            IPublisher publisher)
        {

            _identityContextService = identityContextService;
            _externalSystemAPI = externalSystemAPI;
            _productService = productService;
            _tenantService = tenantService;
            _subscriptionService = subscriptionService;
            _genericAttributeService = genericAttributeService;
            _dbContext = dbContext;
            _publisher = publisher;

        }
        #endregion


        #region varibles  
        private List<PlanFeatureInfoModel> _subscriptionPlanFeatures;
        #endregion
        #region Abstract Props  
        public abstract OrderType OrderType { get; set; }
        public abstract PaymentPurpose PaymentPurpose { get; set; }
        #endregion

        #region Abstract Methods  
        public abstract Task Handle(SubscriptionRenewalPreparationModel model, CancellationToken cancellationToken);
        #endregion

        public virtual async Task<bool> TryRemovingForcedDowngradeAttributesAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var keyGroup = typeof(Subscription).Name;
            var genericAttributes = await _genericAttributeService.GetAttributesForEntityAsync(subscriptionId, keyGroup, cancellationToken);
            var genericAttributesToDelete = genericAttributes.Where(x => x.Key.Equals(Consts.GenericAttributeKey.ForcedDowngrade, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (genericAttributesToDelete.Any())
            {
                _dbContext.GenericAttributes.RemoveRange(genericAttributesToDelete);
                return true;
            }

            return false;
        }

        public virtual async Task<string> GetTenantSystemNameAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            // Unique Name tenant retrieving  
            Expression<Func<Tenant, string>> tenantSelector = x => x.SystemName;
            var tenantResult = await _tenantService.GetByIdAsync(tenantId, tenantSelector, cancellationToken);
            ArgumentNullException.ThrowIfNull(tenantResult.Data);
            return tenantResult.Data;
        }

        public virtual async Task UpdateSubscriptionRenewalStatusAsync(SubscriptionRenewal subscriptionRenewal, SubscriptionRenewalStatus renewalStatus, CancellationToken cancellationToken = default)
        {
            subscriptionRenewal.Status = renewalStatus;
            subscriptionRenewal.ModificationDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<ExternalSystemRequestModel<Model>> BuildRequestModelAsync<Model>(Guid productId,
                                                                                                    Guid tenantId,
                                                                                                    Expression<Func<Product, ExternalSystemApiModel>> selector,
                                                                                                    CancellationToken cancellationToken = default)
        {
            var urlItemResult = await _productService.GetProductEndpointByIdAsync(productId, selector, cancellationToken);
            ArgumentNullException.ThrowIfNull(urlItemResult.Data);
            ArgumentNullException.ThrowIfNull(urlItemResult.Data.Url);
            ArgumentNullException.ThrowIfNull(urlItemResult.Data.ApiKey);
            ApplySubscriptionRenewalByExternalSystemAction = urlItemResult.Data.ApplySubscriptionRenewalByExternalSystemAction;

            return new ExternalSystemRequestModel<Model>
            {
                BaseUrl = urlItemResult.Data.Url,
                ApiKey = urlItemResult.Data.ApiKey,
                TenantId = tenantId,
            };
        }

        public virtual async Task PublishOperationPreparedEventAsync(TenantProcessType tenantProcessType, Subscription subscription, CancellationToken cancellationToken = default)
        {
            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                   processType: tenantProcessType,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: string.Empty,
                                                   processId: out _,
                                                   subscriptions: subscription));
        }
        public SubscriptionRenewalTypeEnum GetSubscriptionRenewalType()
        {
            var attributes = GetType().GetCustomAttributes(typeof(SubscriptionRenewalTypeAttribute), true);

            if (attributes.Length > 0)
            {
                var subscriptionRenewalTypeAttribute = attributes[0] as SubscriptionRenewalTypeAttribute;
                ArgumentNullException.ThrowIfNull(subscriptionRenewalTypeAttribute);
                return subscriptionRenewalTypeAttribute.RenewalType;
            }
            else
            {
                throw new InvalidOperationException($"<{nameof(SubscriptionRenewalTypeAttribute)}> is not applied to {GetType().Name} class.");
            }
        }



    }

}
