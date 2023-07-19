using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetProductTenantsList
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
