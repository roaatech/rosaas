using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class TenantStatusUpdatedEventHandler : IInternalDomainEventHandler<TenantStatusUpdatedEvent>
    {
        private readonly ILogger<TenantStatusUpdatedEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly BackgroundServicesStore _backgroundServicesStore;
        private readonly ITenantWorkflow _workflow;

        public TenantStatusUpdatedEventHandler(IRosasDbContext dbContext,
                                                     IIdentityContextService identityContextService,
                                                     BackgroundServicesStore backgroundServicesStore,
                                                     ITenantWorkflow workflow,
                                                     ILogger<TenantStatusUpdatedEventHandler> logger)
        {
            _identityContextService = identityContextService;
            _backgroundServicesStore = backgroundServicesStore;
            _dbContext = dbContext;
            _workflow = workflow;
            _logger = logger;
        }

        public async Task Handle(TenantStatusUpdatedEvent @event, CancellationToken cancellationToken)
        {
            DateTime date = DateTime.UtcNow;

            var statusHistory = new TenantStatusHistory
            {
                Id = Guid.NewGuid(),
                TenantId = @event.Subscription.TenantId,
                ProductId = @event.Subscription.ProductId,
                SubscriptionId = @event.Subscription.Id,
                Status = @event.Subscription.Status,
                Step = @event.Subscription.Step,
                PreviousStatus = @event.PreviousStatus,
                PreviousStep = @event.PreviousStep,
                OwnerId = _identityContextService.GetActorId(),
                OwnerType = _identityContextService.GetUserType(),
                Created = date,
                TimeStamp = date,
                Message = @event.Workflow.Message,
            };



            if (@event.PreviousStatus == Domain.Enums.TenantStatus.Active)
            {
                statusHistory.AddDomainEvent(new StatusOfActiveTenantIsUpdated(@event.Subscription));
            }


            statusHistory.AddDomainEvent(new TenantProcessingCompletedEvent(
                                                            TenantProcessType.StatusChanged,
                                                            true,
                                                            new TenantStatusChangedProcessedData(@event.Subscription.Status, @event.Subscription.Step, @event.PreviousStatus, @event.PreviousStep).Serialize(),
                                                            @event.Notes,
                                                            out _,
                                                            @event.Subscription));
            if (@event.Workflow.Events is not null)
            {
                foreach (var eventId in @event.Workflow.Events)
                {
                    var workflowEvent = await _workflow.GetWorkflowEventByIdAsync(eventId, cancellationToken);
                    var eventType = JsonConvert.DeserializeObject<Type>(workflowEvent.Type, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var workflowEventInstance = Activator.CreateInstance(eventType,
                                                                         @event.Subscription.TenantId,
                                                                         @event.Subscription.ProductId,
                                                                         @event.Subscription.Id,
                                                                         @event.Subscription.Status,
                                                                         @event.Subscription.Step,
                                                                         @event.PreviousStatus,
                                                                         @event.PreviousStep
                                                                         );

                    var wfEvent = workflowEventInstance as SendingRequestBaseEvent;

                    statusHistory.AddDomainEvent(wfEvent);
                }

            }

            _dbContext.TenantStatusHistory.Add(statusHistory);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _backgroundServicesStore.RemoveTenantProcess(@event.Subscription.TenantId, @event.Subscription.ProductId);
        }
    }
}
