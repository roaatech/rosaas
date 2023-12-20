namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;

public record TenantCreatedResultDto
{
    public TenantCreatedResultDto(Guid id, Guid orderId, IEnumerable<ProductTenantCreatedResultDto> products)
    {
        Id = id;
        OrderId = orderId;
        Products = products;
        HasToPay = false;
    }

    public TenantCreatedResultDto(Guid tenantCreationRequestId)
    {
        TenantCreationRequestId = tenantCreationRequestId;
        HasToPay = true;
    }

    public bool HasToPay { get; set; }
    public Guid TenantCreationRequestId { get; set; }
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public IEnumerable<ProductTenantCreatedResultDto> Products { get; set; }
}
