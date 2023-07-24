using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Commands.UpdateTenantMetadata;
public record UpdateTenantMetadataCommand : IRequest<Result>
{
    public UpdateTenantMetadataCommand(Guid tenantId, Guid productId, dynamic metadata)
    {
        TenantId = tenantId;
        ProductId = productId;
        Metadata = metadata;
    }
    public UpdateTenantMetadataCommand() { }

    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public dynamic Metadata { get; set; }
}