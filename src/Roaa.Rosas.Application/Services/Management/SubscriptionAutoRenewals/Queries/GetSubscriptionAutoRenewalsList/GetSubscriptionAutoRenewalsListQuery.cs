using MediatR;
using Roaa.Rosas.Application.Services.Management.SubscriptionAutoRenewals.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionAutoRenewals.Queries.GetSubscriptionAutoRenewalsList
{
    public record GetSubscriptionAutoRenewalsListQuery : IRequest<Result<List<SubscriptionAutoRenewalDto>>>
    {
        public GetSubscriptionAutoRenewalsListQuery()
        {
        }

    }
}
