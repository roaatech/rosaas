using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantProcessingCompletedEvent<T> : BaseInternalEvent where T : TenantProcessedData
    {
        public List<Subscription> Subscriptions { get; set; } = new();
        public TenantProcessType ProcessType { get; set; }
        public T? ProcessedData { get; set; }
        public bool Enabled { get; set; }
        public Guid ProcessId { get; set; }
        public string Notes { get; set; } = string.Empty;



        public TenantProcessingCompletedEvent(TenantProcessType processType, bool enabled, T? processedData, out Guid processId, params Subscription[] subscriptions)
        {
            Subscriptions = subscriptions.ToList();
            ProcessedData = processedData;
            ProcessType = processType;
            Enabled = enabled;
            processId = Guid.NewGuid();
            ProcessId = processId;
        }


        public TenantProcessingCompletedEvent(TenantProcessType processType, bool enabled, T? processedData, out Guid processId, List<Subscription> subscriptions)
        {
            Subscriptions = subscriptions;
            ProcessedData = processedData;
            ProcessType = processType;
            Enabled = enabled;
            processId = Guid.NewGuid();
            ProcessId = processId;
        }

        public TenantProcessingCompletedEvent(TenantProcessType processType, bool enabled, T? processedData, string notes, out Guid processId, List<Subscription> subscriptions)
        {
            Subscriptions = subscriptions;
            ProcessedData = processedData;
            ProcessType = processType;
            Enabled = enabled;
            processId = Guid.NewGuid();
            ProcessId = processId;
            Notes = notes;
        }


    }
}
