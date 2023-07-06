namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record CreateProductModel
    {
        public Guid ClientId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CreationEndpoint { get; set; } = string.Empty;
        public string ActivationEndpoint { get; set; } = string.Empty;
        public string DeactivationEndpoint { get; set; } = string.Empty;
        public string DeletionEndpoint { get; set; } = string.Empty;
    }
}
