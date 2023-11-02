using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantProcessingCompletedEvent : BaseInternalEvent
    {
        public List<Subscription> Subscriptions { get; set; } = new();
        public TenantProcessType ProcessType { get; set; }
        public string? ProcessedData { get; set; }
        public bool Enabled { get; set; }
        public Guid ProcessId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string SystemComment { get; set; } = string.Empty;



        public TenantProcessingCompletedEvent(TenantProcessType processType, bool enabled, string? processedData, out Guid processId, params Subscription[] subscriptions)
        {
            Subscriptions = subscriptions.ToList();
            ProcessedData = processedData;
            ProcessType = processType;
            Enabled = enabled;
            processId = Guid.NewGuid();
            ProcessId = processId;
        }

        public TenantProcessingCompletedEvent(TenantProcessType processType, bool enabled, string? processedData, string comment, string systemComment, out Guid processId, params Subscription[] subscriptions)
        {
            Subscriptions = subscriptions.ToList();
            ProcessedData = processedData;
            ProcessType = processType;
            Enabled = enabled;
            processId = Guid.NewGuid();
            ProcessId = processId;
            Comment = comment;
            SystemComment = systemComment;
        }

        public TenantProcessingCompletedEvent(TenantProcessType processType, bool enabled, string? processedData, out Guid processId, List<Subscription> subscriptions)
        {
            Subscriptions = subscriptions;
            ProcessedData = processedData;
            ProcessType = processType;
            Enabled = enabled;
            processId = Guid.NewGuid();
            ProcessId = processId;
        }

        public TenantProcessingCompletedEvent(TenantProcessType processType, bool enabled, string? processedData, string comment, string systemComment, out Guid processId, List<Subscription> subscriptions)
        {
            Subscriptions = subscriptions;
            ProcessedData = processedData;
            ProcessType = processType;
            Enabled = enabled;
            processId = Guid.NewGuid();
            ProcessId = processId;
            Comment = comment;
            SystemComment = systemComment;
        }


    }



}
