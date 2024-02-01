using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SendingTenantActivationRequestEventHandler : IInternalDomainEventHandler<SendingTenantActivationRequestEvent>
    {
        private readonly ILogger<SendingTenantActivationRequestEventHandler> _logger;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly IExternalSystemAPI _externalSystemAPI;
        private readonly IProductService _productService;
        private readonly ITenantService _tenantService;

        public SendingTenantActivationRequestEventHandler(
                                                 ITenantWorkflow workflow,
                                                 IIdentityContextService identityContextService,
                                                 IExternalSystemAPI externalSystemAPI,
                                                 IProductService productService,
                                                 ITenantService tenantService,
                                                 ILogger<SendingTenantActivationRequestEventHandler> logger)
        {
            _workflow = workflow;
            _identityContextService = identityContextService;
            _externalSystemAPI = externalSystemAPI;
            _productService = productService;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task Handle(SendingTenantActivationRequestEvent @event, CancellationToken cancellationToken)
        {

            // External System's url preparation
            Expression<Func<Product, ProductApiModel>> selector = x => new ProductApiModel(x.ApiKey, x.ActivationUrl);

            var urlItemResult = await _productService.GetProductEndpointByIdAsync(@event.ProductId, selector, cancellationToken);




            // Unique Name tenant retrieving  
            Expression<Func<Tenant, string>> tenantSelector = x => x.SystemName;

            var tenantResult = await _tenantService.GetByIdAsync(@event.TenantId, tenantSelector, cancellationToken);




            // External System calling to activate the tenant resorces 
            var callingResult = await _externalSystemAPI.ActivateTenantAsync(new ExternalSystemRequestModel<ActivateTenantModel>
            {
                BaseUrl = urlItemResult.Data.Url,
                ApiKey = urlItemResult.Data.ApiKey,
                TenantId = @event.TenantId,
                Data = new()
                {
                    TenantName = tenantResult.Data,
                }
            }, cancellationToken);




            // Getting the next status of the workflow 
            var action = callingResult.Success ? WorkflowAction.Ok : WorkflowAction.Cancel;

            var workflow = await _workflow.GetNextStageAsync(expectedResourceStatus: @event.ExpectedResourceStatus,
                                                                       currentStatus: @event.Status,
                                                                       currentStep: @event.Step,
                                                                       userType: UserType.ExternalSystem,
                                                                       action: action);

            // moving the tenant to the next status of its workflow
            await _tenantService.SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                TenantId = @event.TenantId,
                ProductId = @event.ProductId,
                Status = workflow.NextStatus,
                Step = workflow.NextStep,
                Action = workflow.Action,
                UserType = UserType.ExternalSystem,
                EditorBy = _identityContextService.GetActorId(),
                DispatchedRequest = new DispatchedRequestModel(callingResult.Data.DurationInMillisecond, callingResult.Data.Url, callingResult.Data.SerializedResponseContent),
                ExpectedResourceStatus = null,
            });
        }
    }
}
