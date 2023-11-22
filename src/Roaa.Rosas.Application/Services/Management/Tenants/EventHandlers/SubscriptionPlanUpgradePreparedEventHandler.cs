using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionPlanUpgradePreparedEventHandler : IInternalDomainEventHandler<SubscriptionPlanUpgradePreparedEvent>
    {
        private readonly ILogger<SubscriptionPlanUpgradePreparedEventHandler> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IExternalSystemAPI _externalSystemAPI;
        private readonly IProductService _productService;
        private readonly ITenantService _tenantService;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;

        public SubscriptionPlanUpgradePreparedEventHandler(IIdentityContextService identityContextService,
                                                    IExternalSystemAPI externalSystemAPI,
                                                    IProductService productService,
                                                    ITenantService tenantService,
                                                    IRosasDbContext dbContext,
                                                    IPublisher publisher,
                                                    ILogger<SubscriptionPlanUpgradePreparedEventHandler> logger)
        {
            _identityContextService = identityContextService;
            _externalSystemAPI = externalSystemAPI;
            _productService = productService;
            _tenantService = tenantService;
            _dbContext = dbContext;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(SubscriptionPlanUpgradePreparedEvent @event, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SubscriptionUpgradePrepared,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: string.Empty,
                                                   processId: out _,
                                                   subscriptions: @event.Subscription));




            // External System's url preparation
            Expression<Func<Product, ProductApiModel>> selector = x => new ProductApiModel(x.ApiKey, x.SubscriptionUpgradeUrl);
            var urlItemResult = await _productService.GetProductEndpointByIdAsync(@event.Subscription.ProductId, selector, cancellationToken);

            // Unique Name tenant retrieving  
            Expression<Func<Tenant, string>> tenantSelector = x => x.UniqueName;
            var tenantResult = await _tenantService.GetByIdAsync(@event.Subscription.TenantId, tenantSelector, cancellationToken);

            // External System calling to upgrade the tenant resorces 
            var callingResult = await _externalSystemAPI.UpgradeTenantAsync(
                new ExternalSystemRequestModel<UpgradeTenantModel>
                {
                    BaseUrl = urlItemResult.Data.Url,
                    ApiKey = urlItemResult.Data.ApiKey,
                    TenantId = @event.Subscription.TenantId,
                    Data = new()
                    {
                        TenantName = tenantResult.Data,
                    }
                },
                cancellationToken);


            var subscription = await _dbContext.Subscriptions
                                               .Where(x => x.Id == @event.Subscription.Id)
                                               .SingleOrDefaultAsync();

            subscription.SubscriptionPlanChangeStatus = SubscriptionPlanChangeStatus.InProgress;

            subscription.AddDomainEvent(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SubscriptionUpgradeBeingApplied,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: string.Empty,
                                                   processId: out _,
                                                   subscriptions: @event.Subscription));

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
