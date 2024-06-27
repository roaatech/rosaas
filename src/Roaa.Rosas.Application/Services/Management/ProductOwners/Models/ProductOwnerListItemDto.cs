namespace Roaa.Rosas.Application.Services.Management.ProductOwners.Models
{
    public class ProductOwnerListItemDto
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }


    }
}
