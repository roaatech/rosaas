using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Application.Tenants.Service;
using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;

namespace Roaa.Rosas.Application.Tenants.EventHandlers
{
    public class TenantCreatedInStoreEventHandler : IInternalDomainEventHandler<TenantCreatedInStoreEvent>
    {
        private readonly ILogger<TenantCreatedInStoreEventHandler> _logger;
        private readonly IPublisher _publisher;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantService _tenantService;

        public TenantCreatedInStoreEventHandler(IPublisher publisher,
                                                 ITenantWorkflow workflow,
                                                 IIdentityContextService identityContextService,
                                                 ITenantService tenantService,
                                                 ILogger<TenantCreatedInStoreEventHandler> logger)
        {
            _publisher = publisher;
            _workflow = workflow;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task Handle(TenantCreatedInStoreEvent @event, CancellationToken cancellationToken)
        {
            var process = await _workflow.GetNextProcessActionAsync(@event.Status, _identityContextService.GetUserType());
            var result = await _tenantService.SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                TenantId = @event.Tenant.Id,
                Status = process.NextStatus,
                Action = Domain.Entities.Management.WorkflowAction.Ok,
                UserType = _identityContextService.GetUserType(),
                EditorBy = _identityContextService.UserId,
            });


            if (result.Success)
            {
                foreach (var resultItem in result.Data)
                {
                    var statusManager = TenantStatusManager.FromKey(resultItem.ProductTenant.Status);

                    await statusManager.PublishEventAsync(_publisher, resultItem.ProductTenant, resultItem.Process.CurrentStatus, cancellationToken);
                }
            }
        }
    }
}
