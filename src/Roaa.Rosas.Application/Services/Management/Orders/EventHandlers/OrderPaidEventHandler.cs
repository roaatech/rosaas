using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.EventHandlers
{
    public class OrderPaidEventHandler : IInternalDomainEventHandler<OrderPaidEvent>
    {
        private readonly ILogger<OrderPaidEventHandler> _logger;
        private readonly ITenantWorkflow _workflow;
        private readonly IPublisher _publisher;

        public OrderPaidEventHandler(ITenantWorkflow workflow,
                                     IPublisher publisher,
                                     ILogger<OrderPaidEventHandler> logger)
        {
            _workflow = workflow;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(OrderPaidEvent @event, CancellationToken cancellationToken)
        {
            var workflowEvent = await _workflow.GetOrderWorkflowEventByOrderIntentAsync(@event.OrderIntent, cancellationToken);

            var eventType = JsonConvert.DeserializeObject<Type>(workflowEvent.Type, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var workflowEventInstance = Activator.CreateInstance(eventType, @event.OrderId, @event.CardReferenceId, @event.PaymentPlatform);

            var wfEvent = workflowEventInstance as OrderCompletionAchievedBaseEvent;

            await _publisher.Publish(wfEvent);
        }

    }
}
/*
  public async Task Handle(OrderPaidEvent @event, CancellationToken cancellationToken)
        {
            var subscriptions = await _dbContext.Subscriptions
                                                .Where(x => x.TenantId == @event.TenantId)
                                                .ToListAsync(cancellationToken);

            foreach (var subscription in subscriptions)
            {
                subscription.IsActive = true;
                subscription.ModificationDate = DateTime.UtcNow;
                subscription.ModifiedByUserId = _identityContextService.UserId;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Getting the next status of the workflow  
            var workflow = await _workflow.GetNextStageAsync(expectedResourceStatus: subscriptions[0].ExpectedResourceStatus,
                                                             currentStatus: subscriptions[0].Status,
                                                             currentStep: subscriptions[0].Step,
                                                             userType: _identityContextService.GetUserType());

            if (workflow is not null)
            {
                // moving the tenant to the next status of its workflow
                var result = await _tenantService.SetTenantNextStatusAsync(new SetTenantNextStatusModel
                {
                    TenantId = @event.TenantId,
                    Status = workflow.NextStatus,
                    Step = workflow.NextStep,
                    Action = Domain.Entities.Management.WorkflowAction.Ok,
                    UserType = _identityContextService.GetUserType(),
                    EditorBy = _identityContextService.UserId,
                    ExpectedResourceStatus = null,
                }, cancellationToken);
            }
        }
 */