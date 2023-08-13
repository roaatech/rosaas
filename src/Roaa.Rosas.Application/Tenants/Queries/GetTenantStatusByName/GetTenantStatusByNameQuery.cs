using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantStatusByName
{
    public record GetTenantStatusByNameQuery : IRequest<Result<TenantStatusDto>>
    {
        public GetTenantStatusByNameQuery(string tenantName, Guid productId)
        {
            TenantName = tenantName;
            ProductId = productId;
        }
        public GetTenantStatusByNameQuery() { }

        public string TenantName { get; init; }
        public Guid ProductId { get; set; }

    }
}
