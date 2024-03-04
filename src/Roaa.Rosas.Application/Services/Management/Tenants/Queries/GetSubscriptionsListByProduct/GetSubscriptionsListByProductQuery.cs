using MediatR;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsListByProduct
{
    public record GetSubscriptionsListByProductQuery : IRequest<Result<List<SubscriptionListItemDto>>>
    {
        public GetSubscriptionsListByProductQuery(Guid productId)
        {
            ProductId = productId;
        }

        public Guid ProductId { get; set; }
    }
}
