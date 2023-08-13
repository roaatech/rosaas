using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantProcessesByTenantId
{
    public record GetTenantProcessesByTenantIdQuery : IRequest<Result<List<TenantProcessDto>>>
    {

        public GetTenantProcessesByTenantIdQuery(Guid tenantId, Guid productId)
        {
            TenantId = tenantId;
            ProductId = productId;
        }

        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
    }
}
