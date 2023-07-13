using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Commands.UpdateTenantMetadata;
public record UpdateTenantMetadataCommand : IRequest<Result>
{
    public UpdateTenantMetadataCommand(Guid tenantId, Guid productId, Dictionary<string, string> metadata)
    {
        TenantId = tenantId;
        ProductId = productId;
        Metadata = metadata;
    }
    public UpdateTenantMetadataCommand() { }

    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}