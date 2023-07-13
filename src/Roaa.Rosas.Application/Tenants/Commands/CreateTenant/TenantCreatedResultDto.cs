using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Tenants.Commands.CreateTenant;

public record TenantCreatedResultDto
{
    public TenantCreatedResultDto(Guid id, IEnumerable<ProductTenantCreatedResultDto> products)
    {
        Id = id;
        Products = products;
    }

    public Guid Id { get; set; }
    public IEnumerable<ProductTenantCreatedResultDto> Products { get; set; }
}

public record ProductTenantCreatedResultDto
{
    public ProductTenantCreatedResultDto(Guid productId, TenantStatus status, IEnumerable<ActionResultModel> actions)
    {
        ProductId = productId;
        Status = status;
        Actions = actions;
    }

    public Guid ProductId { get; set; }
    public TenantStatus Status { get; set; }
    public IEnumerable<ActionResultModel> Actions { get; set; } = new List<ActionResultModel>();
}

