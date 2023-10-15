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

        public TenantProcessType ProcessType { get; set; }

        public string Data { get; set; } = string.Empty;

        public Guid? OwnerId { get; set; }

        public UserType OwnerType { get; set; }

        public DateTime ProcessDate { get; set; }

        public DateTime TimeStamp { get; set; }

        public int UpdatesCount { get; set; } = 1;

        public bool Enabled { get; set; } = true;

        public string Notes { get; set; } = string.Empty;
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

    public record TenantProcessedData
    {

    }
    public record TenantMetadataUpdatedProcessedData : TenantProcessedData
    {
        public dynamic UpdatedData { get; set; } = string.Empty;

        public string OldData { get; set; } = string.Empty;
    }
    public record TenantDataUpdatedProcessedData : TenantProcessedData
    {
        public TenantInfoProcessedData UpdatedData { get; set; } = new();

        public TenantInfoProcessedData OldData { get; set; } = new();
    }

    public record SpecificationsUpdatedProcessedData : TenantProcessedData
    {
        public TenantInfoProcessedData UpdatedData { get; set; } = new();

        public TenantInfoProcessedData OldData { get; set; } = new();
    }
    public record TenantInfoProcessedData : TenantProcessedData
    {
        public string Title { get; set; } = string.Empty;
    }
    public record TenantStatusChangedProcessedData : TenantProcessedData
    {
        public TenantStatus Status { get; set; }

        public TenantStatus PreviousStatus { get; set; }

    }
}
