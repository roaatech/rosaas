using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenantSpecifications;
public record UpdateTenantSpecificationsCommand : IRequest<Result>
{
    public UpdateTenantSpecificationsCommand(Guid id, Guid productId, List<UpdateSpecificationValueModel> specifications)
    {
        TenantId = id;
        ProductId = productId;
        Specifications = specifications;
    }

    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public List<UpdateSpecificationValueModel> Specifications { get; set; } = new();
}


public record UpdateSpecificationValueModel
{
    public Guid SpecificationId { get; set; }
    public string Value { get; set; } = string.Empty;
}

public record SpecificationModel
{
    public Guid SpecificationId { get; set; }
    public string SpecificationName { get; set; } = string.Empty;
}