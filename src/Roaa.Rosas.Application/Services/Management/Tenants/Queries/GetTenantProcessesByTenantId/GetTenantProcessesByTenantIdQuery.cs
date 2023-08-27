using MediatR;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantProcessesByTenantId
{
    public record GetTenantProcessesByTenantIdQuery : IRequest<PaginatedResult<TenantProcessDto>>
    {

        public GetTenantProcessesByTenantIdQuery(Guid tenantId, Guid productId, PaginationMetaData paginationInfo)
        {
            TenantId = tenantId;
            ProductId = productId;
            PaginationInfo = paginationInfo;
        }

        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
        public PaginationMetaData PaginationInfo { get; init; } = new();
    }
}
