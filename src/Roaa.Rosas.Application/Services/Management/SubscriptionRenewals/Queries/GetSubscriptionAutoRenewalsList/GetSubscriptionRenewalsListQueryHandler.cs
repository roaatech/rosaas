using MediatR;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Queries.GetSubscriptionAutoRenewalsList
{
    public class GetSubscriptionRenewalsListQueryHandler : IRequestHandler<GetSubscriptionRenewalsListQuery, Result<List<SubscriptionRenewalDto>>>
    {
        #region Props 
        private readonly ISubscriptionRenewalService _subscriptionAutoRenewalService;
        private readonly IOrderService _orderService;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetSubscriptionRenewalsListQueryHandler(ISubscriptionRenewalService subscriptionAutoRenewalService,
                                                IOrderService orderService,
                                                IIdentityContextService identityContextService)
        {
            _subscriptionAutoRenewalService = subscriptionAutoRenewalService;
            _orderService = orderService;
            _identityContextService = identityContextService;
        }
        #endregion


        #region Handler   
        public async Task<Result<List<SubscriptionRenewalDto>>> Handle(GetSubscriptionRenewalsListQuery request, CancellationToken cancellationToken)
        {
            var result = await _subscriptionAutoRenewalService.GetSubscriptionRenewalsListByUserIdAsync(_identityContextService.UserId, cancellationToken);

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
