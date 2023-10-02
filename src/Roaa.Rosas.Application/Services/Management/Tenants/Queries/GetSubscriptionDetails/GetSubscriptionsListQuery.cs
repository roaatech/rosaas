using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionDetails
{
    public record GetSubscriptionDetailsQuery : IRequest<Result<SubscriptionDetailsDto>>
    {
        public GetSubscriptionDetailsQuery(Guid tenantId, Guid productId)
        {
            ProductId = productId;
            TenantId = tenantId;
        }

        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
    }
}
