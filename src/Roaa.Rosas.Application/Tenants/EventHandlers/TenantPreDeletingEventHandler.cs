using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Application.Tenants.Service;
using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Tenants.EventHandlers
{
    public class TenantPreDeletingEventHandler : IInternalDomainEventHandler<TenantPreDeletingEvent>
    {
        private readonly ILogger<TenantPreDeletingEventHandler> _logger;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly IExternalSystemAPI _externalSystemAPI;
        private readonly IProductService _productService;
        private readonly ITenantService _tenantService;

        public TenantPreDeletingEventHandler(
                                            ITenantWorkflow workflow,
                                            IIdentityContextService identityContextService,
                                            IExternalSystemAPI externalSystemAPI,
                                            IProductService productService,
                                            ITenantService tenantService,
                                            ILogger<TenantPreDeletingEventHandler> logger)
        {
            _workflow = workflow;
            _identityContextService = identityContextService;
            _externalSystemAPI = externalSystemAPI;
            _productService = productService;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task Handle(TenantPreDeletingEvent @event, CancellationToken cancellationToken)
        {
            Expression<Func<ProductTenant, ProductUrlListItem>> selector = x => new ProductUrlListItem
            {
                Id = x.ProductId,
                Url = x.Product.DeactivationEndpoint,
            };

            var urlsItemsResult = await _productService.GetProductsUrlsByTenantIdAsync(@event.Tenant.Id, selector, cancellationToken);

            var callingResults = new List<Result<ExternalSystemResultModel<dynamic>>>();

            foreach (var item in urlsItemsResult.Data)
                callingResults.Add(await _externalSystemAPI.DeleteTenantAsync(new ExternalSystemRequestModel<DeleteTenantModel>
                {
                    BaseUrl = item.Url,
                    Data = new()
                    {
                        TenantId = @event.Tenant.Id,
                    }
                }, cancellationToken));

            var action = WorkflowAction.Cancel;

            if (callingResults.Where(x => x.Success).Any())
            {
                action = WorkflowAction.Ok;
            }

            var process = await _workflow.GetNextProcessActionAsync(@event.Tenant.Status, UserType.ExternalSystem, action);
            await _tenantService.ChangeTenantStatusAsync(new ChangeTenantStatusModel
            {
                TenantId = @event.Tenant.Id,
                Status = process.NextStatus,
                Action = process.Action,
                UserType = process.OwnerType,
                EditorBy = _identityContextService.UserId,
            });
        }
    }
}
