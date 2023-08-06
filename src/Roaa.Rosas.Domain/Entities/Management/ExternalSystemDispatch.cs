namespace Roaa.Rosas.Domain.Entities.Management
{
    public class ExternalSystemDispatch : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public bool IsSuccessful { get; set; }

        public string Url { get; set; } = string.Empty;

        public int Duration { get; set; }

        public DateTime DispatchDate { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
}
