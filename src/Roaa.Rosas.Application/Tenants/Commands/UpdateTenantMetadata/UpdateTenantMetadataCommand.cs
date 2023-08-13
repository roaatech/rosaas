using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Commands.UpdateTenantMetadata;
public record UpdateTenantMetadataCommand : IRequest<Result>
{
    public UpdateTenantMetadataCommand(string tenantName, Guid productId, dynamic metadata)
    {
        TenantName = tenantName;
        ProductId = productId;
        Metadata = metadata;
    }
    public UpdateTenantMetadataCommand() { }

    public string TenantName { get; set; }
    public Guid ProductId { get; set; }
    public dynamic Metadata { get; set; }
}