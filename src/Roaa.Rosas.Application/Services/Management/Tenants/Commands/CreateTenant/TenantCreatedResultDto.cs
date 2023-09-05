using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;

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

public record PlanInfoModel
{
    public Guid PlanPriceId { get; set; }
    public Guid PlanId { get; set; }
    public bool IsPublished { get; set; }
    public Cycle PlanCycle { get; set; }
    public ProductUrlListItem Product { get; set; } = new();
    public List<FeatureInfoModel> Features { get; set; } = new();
}

public record FeatureInfoModel
{
    public Guid PlanFeatureId { get; set; }
    public Guid PlanId { get; set; }
    public Guid FeatureId { get; set; }
    public int? Limit { get; set; }
    public FeatureType Type { get; set; }
    public FeatureUnit? Unit { get; set; }
    public FeatureReset Reset { get; set; }
}