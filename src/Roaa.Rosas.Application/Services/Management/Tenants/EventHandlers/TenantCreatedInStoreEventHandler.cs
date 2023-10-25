using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.Specifications;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Settings;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class TenantCreatedInStoreEventHandler : IInternalDomainEventHandler<TenantCreatedInStoreEvent>
    {
        private readonly ILogger<TenantCreatedInStoreEventHandler> _logger;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantService _tenantService;
        private readonly ISpecificationService _specificationService;
        private readonly ISettingService _settingService;

        public TenantCreatedInStoreEventHandler(ITenantWorkflow workflow,
                                                IIdentityContextService identityContextService,
                                                ITenantService tenantService,
                                                ISpecificationService specificationService,
                                                ISettingService settingService,
                                                ILogger<TenantCreatedInStoreEventHandler> logger)
        {
            _workflow = workflow;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
            _specificationService = specificationService;
            _settingService = settingService;
            _logger = logger;
        }

        public async Task Handle(TenantCreatedInStoreEvent @event, CancellationToken cancellationToken)
        {
            // Setting the published specifications of the tenant's products as subscribed
            await _specificationService.SetSpecificationsAsSubscribedAsync(@event.Tenant.Id, cancellationToken);

            var settings = (await _settingService.LoadSettingAsync<TenantSettings>(cancellationToken)).Data;

            if (settings.SendCreationRequestAutomaticallyAfterTenantCreatedInStore)
            {
                await SetTenantNextStatusAsync(@event, cancellationToken);
            }

        }

        private async Task SetTenantNextStatusAsync(TenantCreatedInStoreEvent @event, CancellationToken cancellationToken)
        {
            // Getting the next status of the workflow  
            var workflow = await _workflow.GetNextStageAsync(expectedResourceStatus: @event.ExpectedResourceStatus,
                                                                       currentStatus: @event.Status,
                                                                     currentStep: @event.Step,
                                                                     userType: _identityContextService.GetUserType());

            // moving the tenant to the next status of its workflow
            var result = await _tenantService.SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                TenantId = @event.Tenant.Id,
                Status = workflow.NextStatus,
                Step = workflow.NextStep,
                Action = Domain.Entities.Management.WorkflowAction.Ok,
                UserType = _identityContextService.GetUserType(),
                EditorBy = _identityContextService.UserId,
            }, cancellationToken);
        }


    }
}
