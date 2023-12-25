using Roaa.Rosas.Application.Services.Management.Products.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;

public record ProductDataModel : ProductUrlListItem
{
    public string SystemName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
