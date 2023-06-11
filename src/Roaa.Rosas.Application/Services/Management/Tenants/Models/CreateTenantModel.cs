namespace Roaa.Rosas.Application.Services.Management.Tenants.Models
{
    public record CreateTenantModel
    {
        public CreateTenantModel(CreateTenantByExternalSystemModel model, params Guid[] productsIds)
        {
            ProductsIds = productsIds.ToList();
            UniqueName = model.UniqueName;
            Title = model.Title;
        }

        public CreateTenantModel()
        {
        }

        public List<Guid> ProductsIds { get; set; } = new();
        public string UniqueName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
    public record CreateTenantByExternalSystemModel
    {
        public string UniqueName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
