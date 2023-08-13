namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record ExternalSystemRequestModel<T>
    {
        public T Data { get; set; }
        public Guid TenantId { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
