using MediatR;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsListByProduct
{
    public class GetSubscriptionsListByProductQueryHandler : IRequestHandler<GetSubscriptionsListByProductQuery, Result<List<SubscriptionListItemDto>>>
    {
        #region Props 
        private readonly ISubscriptionService _subscriptionService;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetSubscriptionsListByProductQueryHandler(ISubscriptionService subscriptionService,
                                               IIdentityContextService identityContextService)
        {
            _subscriptionService = subscriptionService;
            _identityContextService = identityContextService;
        }
        #endregion


        #region Handler   
        public async Task<Result<List<SubscriptionListItemDto>>> Handle(GetSubscriptionsListByProductQuery request, CancellationToken cancellationToken)
        {
            return await _subscriptionService.GetSubscriptionsListByProductIdAsync(request.ProductId, cancellationToken);
        }
        #endregion
    }
}
