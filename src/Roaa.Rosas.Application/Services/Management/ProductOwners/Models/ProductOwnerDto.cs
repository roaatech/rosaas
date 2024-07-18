using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Application.Services.Management.ProductOwners.Models
{
    public class ProductOwnerDto
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public List<CustomLookupItemDto<Guid>> Products { get; set; } = new List<CustomLookupItemDto<Guid>>();

    }
}
