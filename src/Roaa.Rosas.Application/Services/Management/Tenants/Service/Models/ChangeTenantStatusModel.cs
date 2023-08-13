using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service.Models
{
    public record ChangeTenantStatusModel
    {
        public ChangeTenantStatusModel(Guid tenantId, TenantStatus status, Guid? productId)
        {
            TenantId = tenantId;
            ProductId = productId;
            Status = status;
        }
        public ChangeTenantStatusModel() { }

        public Guid TenantId { get; init; }
        public Guid? ProductId { get; init; }
        public TenantStatus Status { get; init; }
    }
}
