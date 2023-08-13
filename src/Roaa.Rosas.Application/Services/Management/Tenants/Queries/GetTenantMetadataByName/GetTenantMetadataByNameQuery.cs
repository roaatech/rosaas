using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantMetadataByName
{
    public record GetTenantMetadataByNameQuery : IRequest<Result<TenantMetadataModel>>
    {
        public GetTenantMetadataByNameQuery(string tenantName, Guid productId)
        {
            TenantName = tenantName;
            ProductId = productId;
        }

        public string TenantName { get; set; }
        public Guid ProductId { get; set; }
    }
}
