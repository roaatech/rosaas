using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record ProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DefaultHealthCheckUrl { get; set; } = string.Empty;
        public LookupItemDto<Guid> Client { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
}
