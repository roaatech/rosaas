using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Specifications;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class TenantCreatedInStoreEventHandler : IInternalDomainEventHandler<TenantCreatedInStoreEvent>
    {
        private readonly ILogger<TenantCreatedInStoreEventHandler> _logger;
        private readonly IPublisher _publisher;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantService _tenantService;
        private readonly ISpecificationService _specificationService;

        public TenantCreatedInStoreEventHandler(IPublisher publisher,
                                                 ITenantWorkflow workflow,
                                                 IIdentityContextService identityContextService,
                                                 ITenantService tenantService,
                                                 ISpecificationService specificationService,
                                                 ILogger<TenantCreatedInStoreEventHandler> logger)
        {
            _publisher = publisher;
            _workflow = workflow;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
            _specificationService = specificationService;
            _logger = logger;
        }

        public async Task Handle(TenantCreatedInStoreEvent @event, CancellationToken cancellationToken)
        {

            await _specificationService.SetSpecificationsAsSubscribedAsync(@event.Tenant.Id, cancellationToken);

            // Getting the next status of the workflow 
            var workflow = await _workflow.GetNextProcessActionAsync(@event.Status, _identityContextService.GetUserType());




            // moving the tenant to the next status of its workflow
            var result = await _tenantService.SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                TenantId = @event.Tenant.Id,
                Status = workflow.NextStatus,
                Action = Domain.Entities.Management.WorkflowAction.Ok,
                UserType = _identityContextService.GetUserType(),
                EditorBy = _identityContextService.UserId,
            }, cancellationToken);


            if (result.Success)
            {
                foreach (var resultItem in result.Data)
                {
                    // Tenant's status manager's finding 
                    var statusManager = TenantStatusManager.FromKey(resultItem.ProductTenant.Status);

                    // Event triggering 
                    await statusManager.PublishEventAsync(_publisher, resultItem.ProductTenant, resultItem.Process.CurrentStatus, cancellationToken);
                }
            }
        }
    }
}
