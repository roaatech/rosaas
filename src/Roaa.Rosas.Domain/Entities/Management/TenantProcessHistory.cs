using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantProcessHistory : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public TenantStatus Status { get; set; }

        public TenantProcessType ProcessType { get; set; }

        public string Data { get; set; } = string.Empty;

        public Guid? OwnerId { get; set; }

        public UserType OwnerType { get; set; }

        public DateTime ProcessDate { get; set; }

        public DateTime TimeStamp { get; set; }

        public bool Enabled { get; set; } = true;
    }

    public enum TenantProcessType
    {
        RecordCreated = 1,
        DataUpdated,
        MetadataUpdated,
        StatusChanged,
        HealthyStatus,
        UnhealthStatus,
        ExternalSystemSuccessfullyInformed,
        FailedToInformExternalSystem,

    }

    public class TenantProcessData
    {

        public class TenantMetadataUpdatedProcessData
        {
            public dynamic UpdatedData { get; set; } = string.Empty;

            public string OldData { get; set; } = string.Empty;
        }
        public class TenantDataUpdatedProcessData
        {
            public TenantInfoProcessData UpdatedData { get; set; } = new();

            public TenantInfoProcessData OldData { get; set; } = new();
        }
        public class TenantInfoProcessData
        {
            public string Title { get; set; } = string.Empty;
        }
        public class TenantStatusChangedProcessData
        {
            public TenantStatus Status { get; set; }

            public TenantStatus PreviousStatus { get; set; }

        }

    }
}
