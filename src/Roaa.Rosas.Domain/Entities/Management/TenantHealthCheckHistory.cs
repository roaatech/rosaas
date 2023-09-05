namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantHealthCheckHistory : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public Guid SubscriptionId { get; set; }

        public bool IsHealthy { get; set; }

        public int Duration { get; set; }

        public string HealthCheckUrl { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public DateTime TimeStamp { get; set; }

    }
}
