using MediatR;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsList
{
    public record GetSubscriptionsListQuery : IRequest<Result<List<MySubscriptionListItemDto>>>
    {
        public GetSubscriptionsListQuery()
        {
        }

    }
}
