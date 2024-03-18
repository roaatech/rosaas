using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.SubscriptionPlansChanging;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionUpgrade;

public class RequestSubscriptionUpgradeCommandHandler : IRequestHandler<RequestSubscriptionUpgradeCommand, Result>
{
    #region Props 
    private readonly ILogger<RequestSubscriptionUpgradeCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionPlanChangingService _subscriptionPlanChangingService;
    #endregion



    #region Corts
    public RequestSubscriptionUpgradeCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionPlanChangingService subscriptionPlanChangingService,
                                                    ILogger<RequestSubscriptionUpgradeCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionPlanChangingService = subscriptionPlanChangingService;
        _logger = logger;
    }
    #endregion



    #region Handler   
    public async Task<Result> Handle(RequestSubscriptionUpgradeCommand command, CancellationToken cancellationToken)
    {
        return await _subscriptionPlanChangingService.CreateSubscriptionUpgradeAsync(command.SubscriptionId,
                                                                                     command.PlanId,
                                                                                     command.PlanPriceId,
                                                                                     command.CardReferenceId,
                                                                                     command.PaymentPlatform,
                                                                                     command.Comment, cancellationToken);
    }
    #endregion
}

