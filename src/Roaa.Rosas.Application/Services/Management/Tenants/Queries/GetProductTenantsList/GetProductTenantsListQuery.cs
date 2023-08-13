using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetProductTenantsList
{
    public record GetProductTenantsListQuery : IRequest<Result<List<ProductTenantListItemDto>>>
    {
        public GetProductTenantsListQuery(Guid productId)
        {
            ProductId = productId;
        }

        public Guid ProductId { get; set; }
    }
}
