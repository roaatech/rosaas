using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Application.Tenants.Service;
using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Tenants.EventHandlers
{
    public class TenantPreDeactivatingEventHandler : IInternalDomainEventHandler<TenantPreDeactivatingEvent>
    {
        private readonly ILogger<TenantPreDeactivatingEventHandler> _logger;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly IExternalSystemAPI _externalSystemAPI;
        private readonly IProductService _productService;
        private readonly ITenantService _tenantService;

        public TenantPreDeactivatingEventHandler(
                                                 ITenantWorkflow workflow,
                                                 IIdentityContextService identityContextService,
                                                 IExternalSystemAPI externalSystemAPI,
                                                 IProductService productService,
                                                 ITenantService tenantService,
                                                 ILogger<TenantPreDeactivatingEventHandler> logger)
        {
            _workflow = workflow;
            _identityContextService = identityContextService;
            _externalSystemAPI = externalSystemAPI;
            _productService = productService;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task Handle(TenantPreDeactivatingEvent @event, CancellationToken cancellationToken)
        {
            Expression<Func<Product, ProductApiModel>> selector = x => new ProductApiModel(x.ApiKey, x.DeactivationUrl);

            var urlItemResult = await _productService.GetProductEndpointByIdAsync(@event.ProductTenant.ProductId, selector, cancellationToken);

            Expression<Func<Tenant, string>> tenantSelector = x => x.UniqueName;

            var tenantResult = await _tenantService.GetByIdAsync(@event.ProductTenant.TenantId, tenantSelector, cancellationToken);

            var callingResult = await _externalSystemAPI.DeactivateTenantAsync(new ExternalSystemRequestModel<DeactivateTenantModel>
            {
                BaseUrl = urlItemResult.Data.Url,
                ApiKey = urlItemResult.Data.ApiKey,
                TenantId = @event.ProductTenant.TenantId,
                Data = new()
                {
                    TenantName = tenantResult.Data,
                }
            }, cancellationToken);

            var action = callingResult.Success ? WorkflowAction.Ok : WorkflowAction.Cancel;

            var process = await _workflow.GetNextProcessActionAsync(@event.ProductTenant.Status, UserType.ExternalSystem, action);

            await _tenantService.SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                TenantId = @event.ProductTenant.TenantId,
                ProductId = @event.ProductTenant.ProductId,
                Status = process.NextStatus,
                Action = process.Action,
                UserType = process.OwnerType,
                EditorBy = _identityContextService.UserId,
            });
        }
    }
}
