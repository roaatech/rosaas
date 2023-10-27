using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenentByNameAndProductId
{
    public record GetTenentByNameAndProductIdQuery : IRequest<Result<ProductTenantDto>>
    {
        public GetTenentByNameAndProductIdQuery(string tenantName, Guid productId)
        {
            TenantName = tenantName;
            ProductId = productId;
        }

        public string TenantName { get; init; }
        public Guid ProductId { get; init; }
    }
}
