using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantProcessHistory : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public Guid SubscriptionId { get; set; }

        public TenantStatus Status { get; set; }

        public TenantStep Step { get; set; }

        public TenantProcessType ProcessType { get; set; }

        public string? Data { get; set; }

        public Guid? OwnerId { get; set; }

        public UserType OwnerType { get; set; }

        public DateTime ProcessDate { get; set; }

        public DateTime TimeStamp { get; set; }

        public int UpdatesCount { get; set; } = 1;

        public bool Enabled { get; set; } = true;

        public string? Notes { get; set; }
    }

    public enum TenantProcessType
    {
        RecordCreated = 1,
        DataUpdated,
        MetadataUpdated,
        StatusChanged,
        HealthyStatus,
        UnhealthyStatus,
        ExternalSystemSuccessfullyInformed,
        FailedToInformExternalSystem,
        SuspendingThePaymentStatusForTenantSubscriptionDueToNonRenewalOfTheSubscription,
        SpecificationsUpdated,

    }



    public class TenantMetadataUpdatedProcessedData : BaseTenantProcessedData
    {

        public dynamic UpdatedData { get; set; } = string.Empty;

        public dynamic OldData { get; set; } = string.Empty;

        public TenantMetadataUpdatedProcessedData(dynamic updatedData, string oldData)
        {
            UpdatedData = updatedData;

            OldData = string.IsNullOrWhiteSpace(oldData) ? null : System.Text.Json.JsonSerializer.Deserialize<dynamic>(oldData);
        }


        public override string Serialize()
        {
            return Serialize(this);
        }
    }

    public class TenantDataUpdatedProcessedData : BaseTenantProcessedData
    {

        public TenantInfoProcessedDataModel UpdatedData { get; set; } = new(string.Empty);

        public TenantInfoProcessedDataModel OldData { get; set; } = new(string.Empty);


        public TenantDataUpdatedProcessedData(TenantInfoProcessedDataModel updatedData, TenantInfoProcessedDataModel oldData)
        {
            UpdatedData = updatedData;
            OldData = oldData;
        }

        public override string Serialize()
        {
            return Serialize(this);
        }
    }

    public class SpecificationsUpdatedProcessedData : BaseTenantProcessedData
    {
        public TenantInfoProcessedDataModel UpdatedData { get; set; } = new(string.Empty);

        public TenantInfoProcessedDataModel OldData { get; set; } = new(string.Empty);


        public SpecificationsUpdatedProcessedData(TenantInfoProcessedDataModel updatedData, TenantInfoProcessedDataModel oldData)
        {
            UpdatedData = updatedData;
            OldData = oldData;
        }

        public override string Serialize()
        {
            return Serialize(this);
        }
    }

    public class TenantStatusChangedProcessedData : BaseTenantProcessedData
    {

        public TenantStatusChangedAsLabelsProcessedDataModel Label { get; set; } = new();

        public TenantStatusChangedAsEnumsProcessedDataModel Enum { get; set; } = new();




        public TenantStatusChangedProcessedData(TenantStatus status, TenantStep step, TenantStatus previousStatus, TenantStep previousStep)
        {

            Label = new TenantStatusChangedAsLabelsProcessedDataModel
            {
                Status = status.ToString(),
                Step = step.ToString(),
                PreviousStatus = previousStatus.ToString(),
                PreviousStep = previousStep.ToString(),
            };
            Enum = new TenantStatusChangedAsEnumsProcessedDataModel
            {
                Status = status,
                Step = step,
                PreviousStatus = previousStatus,
                PreviousStep = previousStep,
            };
        }

        public override string Serialize()
        {
            return Serialize(this);
        }
    }



    public abstract class BaseTenantProcessedData
    {
        public abstract string Serialize();
        public string Serialize(dynamic data)
        {
            return System.Text.Json.JsonSerializer.Serialize(data);
        }
    }

    public class TenantInfoProcessedDataModel
    {


        public string Title { get; set; } = string.Empty;

        public TenantInfoProcessedDataModel(string title)
        {
            Title = title;
        }
    }

    public class TenantStatusChangedAsLabelsProcessedDataModel
    {
        public string Status { get; set; } = string.Empty;

        public string Step { get; set; } = string.Empty;

        public string PreviousStatus { get; set; } = string.Empty;

        public string PreviousStep { get; set; } = string.Empty;
    }

    public class TenantStatusChangedAsEnumsProcessedDataModel
    {
        public TenantStatus Status { get; set; }

        public TenantStep Step { get; set; }

        public TenantStatus PreviousStatus { get; set; }

        public TenantStep PreviousStep { get; set; }
    }
}
