using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Models.TenantProcessHistoryData
{
    public class ProcessedDataOfTenantStatusModel : BaseProcessedDataOfTenant
    {
        public ProcessedDispatchedRequestModel? DispatchedRequest { get; set; }

        public dynamic? ReceivedRequest { get; set; }

        public ProcessedTenantStatusModel Status { get; set; }

        public ProcessedDataOfTenantStatusModel(TenantStatus status,
                                                TenantStep step,
                                                TenantStatus previousStatus,
                                                TenantStep previousStep,
                                                DispatchedRequestModel? dispatchedRequest,
                                                dynamic? receivedRequest)
        {
            Status = new ProcessedTenantStatusModel(status, step, previousStatus, previousStep);
            DispatchedRequest = Map(dispatchedRequest);
            ReceivedRequest = receivedRequest;
        }

        public override string Serialize()
        {
            return Serialize(this);
        }
    }



    public class ProcessedTenantStatusModel
    {

        public ProcessedTenantStatusAsLabelsModel Label { get; set; } = new();

        public ProcessedTenantStatusAsEnumsModel Enum { get; set; } = new();




        public ProcessedTenantStatusModel(TenantStatus status, TenantStep step, TenantStatus previousStatus, TenantStep previousStep)
        {

            Label = new ProcessedTenantStatusAsLabelsModel
            {
                Status = status.ToString(),
                Step = step.ToString(),
                PreviousStatus = previousStatus.ToString(),
                PreviousStep = previousStep.ToString(),
            };
            Enum = new ProcessedTenantStatusAsEnumsModel
            {
                Status = status,
                Step = step,
                PreviousStatus = previousStatus,
                PreviousStep = previousStep,
            };
        }
    }
    public class ProcessedTenantStatusAsLabelsModel
    {
        public string Status { get; set; } = string.Empty;

        public string Step { get; set; } = string.Empty;

        public string PreviousStatus { get; set; } = string.Empty;

        public string PreviousStep { get; set; } = string.Empty;
    }

    public class ProcessedTenantStatusAsEnumsModel
    {
        public TenantStatus Status { get; set; }

        public TenantStep Step { get; set; }

        public TenantStatus PreviousStatus { get; set; }

        public TenantStep PreviousStep { get; set; }
    }

    public class ProcessedDispatchedRequestModel
    {
        public double DurationInMillisecond { get; set; }
        public string RequestUrl { get; set; } = string.Empty;
        public dynamic ResponseContent { get; set; } = string.Empty;
    }
}
