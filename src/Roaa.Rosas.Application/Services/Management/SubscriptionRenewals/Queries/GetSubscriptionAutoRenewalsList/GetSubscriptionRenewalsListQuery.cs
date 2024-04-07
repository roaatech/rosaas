using MediatR;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Queries.GetSubscriptionAutoRenewalsList
{
    public record GetSubscriptionRenewalsListQuery : IRequest<Result<List<SubscriptionRenewalDto>>>
    {
        public GetSubscriptionRenewalsListQuery()
        {
        }

    }
}
