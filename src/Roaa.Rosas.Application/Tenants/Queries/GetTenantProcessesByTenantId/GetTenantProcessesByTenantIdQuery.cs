using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantProcessesByTenantId
{
    public record GetTenantProcessesByTenantIdQuery : IRequest<Result<List<TenantProcessDto>>>
    {
        public GetTenantProcessesByTenantIdQuery(Guid tenantId)
        {
            TenantId = tenantId;
        }

        public Guid TenantId { get; set; }
    }
}
