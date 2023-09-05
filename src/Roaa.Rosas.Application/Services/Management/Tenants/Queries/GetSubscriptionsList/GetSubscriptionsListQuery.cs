using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsList
{
    public record GetSubscriptionsListQuery : IRequest<Result<List<SubscriptionListItemDto>>>
    {
        public GetSubscriptionsListQuery(Guid productId)
        {
            ProductId = productId;
        }

        public Guid ProductId { get; set; }
    }
}
