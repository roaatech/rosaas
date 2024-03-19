using MediatR;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.SubscriptionAutoRenewals;
using Roaa.Rosas.Application.Services.Management.SubscriptionAutoRenewals.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.Queries.GetSubscriptionsList
{
    public class GetSubscriptionAutoRenewalsListQueryHandler : IRequestHandler<GetSubscriptionAutoRenewalsListQuery, Result<List<SubscriptionAutoRenewalDto>>>
    {
        #region Props 
        private readonly ISubscriptionAutoRenewalService _subscriptionAutoRenewalService;
        private readonly IOrderService _orderService;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetSubscriptionAutoRenewalsListQueryHandler(ISubscriptionAutoRenewalService subscriptionAutoRenewalService,
                                                IOrderService orderService,
                                                IIdentityContextService identityContextService)
        {
            _subscriptionAutoRenewalService = subscriptionAutoRenewalService;
            _orderService = orderService;
            _identityContextService = identityContextService;
        }
        #endregion


        #region Handler   
        public async Task<Result<List<SubscriptionAutoRenewalDto>>> Handle(GetSubscriptionAutoRenewalsListQuery request, CancellationToken cancellationToken)
        {
            var result = await _subscriptionAutoRenewalService.GetSubscriptionAutoRenewalsListByUserIdAsync(_identityContextService.UserId, cancellationToken);

            if (result.Data != null && result.Data.Any())
            {
                var cards = await _orderService.GetPaymentMethodCardsListAsync(result.Data.Select(x => new Guid?(x.Subscription.Id)).ToList(), cancellationToken);

                foreach (var subscription in result.Data)
                {
                    subscription.PaymentMethodCard = cards.Where(x => x.Key == subscription.Id).Select(x => x.Value).FirstOrDefault();
                }
            }

            return result;
        }

        #endregion
    }
}
