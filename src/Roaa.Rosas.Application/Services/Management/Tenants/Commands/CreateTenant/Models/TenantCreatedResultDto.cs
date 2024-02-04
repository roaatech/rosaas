namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;

public record TenantCreatedResultDto
{
    public TenantCreatedResultDto(Guid tenantId, string tenantSystemName, IEnumerable<ProductTenantCreatedResultDto> products)
    {
        TenantId = tenantId;
        TenantSystemName = tenantSystemName;
        Products = products;
    }


    public string TenantSystemName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public IEnumerable<ProductTenantCreatedResultDto> Products { get; set; }
}
