namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record UpdateProductModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
