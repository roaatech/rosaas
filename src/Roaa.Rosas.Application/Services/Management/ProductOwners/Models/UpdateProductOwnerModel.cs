namespace Roaa.Rosas.Application.Services.Management.ProductOwners.Models
{
    public class UpdateProductOwnerModel
    {
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted;
    }
}
