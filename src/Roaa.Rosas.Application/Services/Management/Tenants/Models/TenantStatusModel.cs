namespace Roaa.Rosas.Application.Services.Management.Tenants.Models
{
    public record TenantStatusModel
    {
        public TenantStatusModel(Guid id, Guid productId)
        {
            Id = id;
            ProductId = productId;
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

    }
}
