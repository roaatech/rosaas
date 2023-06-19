using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Commands.CreateTenant;

public record CreateTenantCommand : IRequest<Result<TenantCreatedResultDto>>
{
    public CreateTenantCommand(CreateTenantByExternalSystemModel model, params Guid[] productsIds)
    {
        ProductsIds = productsIds.ToList();
        UniqueName = model.UniqueName;
        Title = model.Title;
    }

    public CreateTenantCommand()
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