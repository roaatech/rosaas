using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantMetadataById
{
    public record GetTenantMetadataByIdQuery : IRequest<Result<TenantMetadataModel>>
    {
        public GetTenantMetadataByIdQuery(Guid tenantId, Guid productId)
        {
            TenantId = tenantId;
            ProductId = productId;
        }

        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
    }
}
