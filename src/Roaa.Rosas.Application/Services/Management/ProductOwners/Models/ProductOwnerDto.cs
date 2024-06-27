using Roaa.Rosas.Application.Services.Management.Products.Models;

namespace Roaa.Rosas.Application.Services.Management.ProductOwners.Models
{
    public class ProductOwnerDto
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public List<ProductListItemDto> Products { get; set; } = new List<ProductListItemDto>();

    }
}
