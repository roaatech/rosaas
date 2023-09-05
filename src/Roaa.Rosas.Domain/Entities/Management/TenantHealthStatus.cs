namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantHealthStatus : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public Guid SubscriptionId { get; set; }

        public bool IsHealthy { get; set; }

        public string HealthCheckUrl { get; set; } = string.Empty;

        public int Duration { get; set; }

        public DateTime LastCheckDate { get; set; }

        public DateTime CheckDate { get; set; }

        public int HealthyCount { get; set; }

        public int UnhealthyCount { get; set; }

        public virtual Subscription? Subscription { get; set; }
    }

}
