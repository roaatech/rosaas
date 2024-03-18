using MediatR;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.Queries.GetSubscriptionsList
{
    public class GetSubscriptionsListQueryHandler : IRequestHandler<GetSubscriptionsListQuery, Result<List<MySubscriptionListItemDto>>>
    {
        #region Props 
        private readonly ISubscriptionService _subscriptionService;
        private readonly IOrderService _orderService;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetSubscriptionsListQueryHandler(ISubscriptionService subscriptionService,
                                                IOrderService orderService,
                                                IIdentityContextService identityContextService)
        {
            _subscriptionService = subscriptionService;
            _orderService = orderService;
            _identityContextService = identityContextService;
        }
        #endregion


        #region Handler   
        public async Task<Result<List<MySubscriptionListItemDto>>> Handle(GetSubscriptionsListQuery request, CancellationToken cancellationToken)
        {
            var result = await _subscriptionService.GetSubscriptionsListByUserIdAsync(_identityContextService.UserId, cancellationToken);

            if (result.Data != null && result.Data.Any())
            {
                var cards = await _orderService.GetPaymentMethodCardsListAsync(result.Data.Select(x => new Guid?(x.SubscriptionId)).ToList(), cancellationToken);

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
