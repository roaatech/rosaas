namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantHealthCheck : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public bool IsHealthy { get; set; }

        public int Duration { get; set; }

        public string HealthCheckUrl { get; set; } = string.Empty;

        public DateTime Created { get; set; }

    }
}
