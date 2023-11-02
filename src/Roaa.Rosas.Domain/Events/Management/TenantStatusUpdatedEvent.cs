using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantStatusUpdatedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; }
        public Workflow Workflow { get; set; }
        public TenantStatus PreviousStatus { get; set; }
        public TenantStep PreviousStep { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string SystemComment { get; set; } = string.Empty;
        public DispatchedRequestModel? DispatchedRequest { get; init; }
        public ReceivedRequestModel? ReceivedRequest { get; init; }


        public TenantStatusUpdatedEvent(Subscription subscription,
                                        Workflow workflow,
                                        TenantStatus previousStatus,
                                        TenantStep previousStep,
                                        string comment,
                                        string systemComment,
                                        DispatchedRequestModel? dispatchedRequest,
                                     ReceivedRequestModel? receivedRequest)
        {
            Subscription = subscription;
            Workflow = workflow;
            PreviousStatus = previousStatus;
            PreviousStep = previousStep;
            Comment = comment;
            SystemComment = systemComment;
            DispatchedRequest = dispatchedRequest;
            ReceivedRequest = receivedRequest;
        }
    }
}
