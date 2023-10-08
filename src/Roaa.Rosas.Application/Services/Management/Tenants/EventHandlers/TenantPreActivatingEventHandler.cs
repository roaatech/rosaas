﻿using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class TenantPreActivatingEventHandler : IInternalDomainEventHandler<TenantPreActivatingEvent>
    {
        private readonly ILogger<TenantPreActivatingEventHandler> _logger;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly IExternalSystemAPI _externalSystemAPI;
        private readonly IProductService _productService;
        private readonly ITenantService _tenantService;

        public TenantPreActivatingEventHandler(
                                                 ITenantWorkflow workflow,
                                                 IIdentityContextService identityContextService,
                                                 IExternalSystemAPI externalSystemAPI,
                                                 IProductService productService,
                                                 ITenantService tenantService,
                                                 ILogger<TenantPreActivatingEventHandler> logger)
        {
            _workflow = workflow;
            _identityContextService = identityContextService;
            _externalSystemAPI = externalSystemAPI;
            _productService = productService;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task Handle(TenantPreActivatingEvent @event, CancellationToken cancellationToken)
        {

            // External System's url preparation
            Expression<Func<Product, ProductApiModel>> selector = x => new ProductApiModel(x.ApiKey, x.ActivationUrl);

            var urlItemResult = await _productService.GetProductEndpointByIdAsync(@event.ProductTenant.ProductId, selector, cancellationToken);




            // Unique Name tenant retrieving  
            Expression<Func<Tenant, string>> tenantSelector = x => x.UniqueName;

            var tenantResult = await _tenantService.GetByIdAsync(@event.ProductTenant.TenantId, tenantSelector, cancellationToken);




            // External System calling to activate the tenant resorces 
            var callingResult = await _externalSystemAPI.ActivateTenantAsync(new ExternalSystemRequestModel<ActivateTenantModel>
            {
                BaseUrl = urlItemResult.Data.Url,
                ApiKey = urlItemResult.Data.ApiKey,
                TenantId = @event.ProductTenant.TenantId,
                Data = new()
                {
                    TenantName = tenantResult.Data,
                }
            }, cancellationToken);




            // Getting the next status of the workflow 
            var action = callingResult.Success ? WorkflowAction.Ok : WorkflowAction.Cancel;

            var workflow = await _workflow.GetNextProcessActionAsync(@event.ProductTenant.Status, UserType.ExternalSystem, action);




            // moving the tenant to the next status of its workflow
            await _tenantService.SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                TenantId = @event.ProductTenant.TenantId,
                ProductId = @event.ProductTenant.ProductId,
                Status = workflow.NextStatus,
                Action = workflow.Action,
                UserType = workflow.OwnerType,
                EditorBy = _identityContextService.UserId,
            });
        }
    }
}
