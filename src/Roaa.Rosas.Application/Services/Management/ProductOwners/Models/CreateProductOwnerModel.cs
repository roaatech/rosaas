namespace Roaa.Rosas.Application.Services.Management.ProductOwners.Models
{
    public class CreateProductOwnerModel
    {
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted = false;
    }
}
