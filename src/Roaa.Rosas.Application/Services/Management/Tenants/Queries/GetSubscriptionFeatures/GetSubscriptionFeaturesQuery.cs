using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionFeatures
{
    public record GetSubscriptionFeaturesQuery : IRequest<Result<List<SubscriptionFeatureDto>>>
    {
        public GetSubscriptionFeaturesQuery(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; set; }
    }
}
