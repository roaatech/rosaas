namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record ProductUrlListItem
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
