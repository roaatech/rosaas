using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionCycles
{
    public record GetSubscriptionCyclesQuery : IRequest<Result<List<SubscriptionCycleDto>>>
    {
        public GetSubscriptionCyclesQuery(Guid subscriptionId, Guid? subscriptionCycleId)
        {
            SubscriptionId = subscriptionId;
            SubscriptionCycleId = subscriptionCycleId;
        }

        public Guid SubscriptionId { get; set; }
        public Guid? SubscriptionCycleId { get; set; }
    }
}
