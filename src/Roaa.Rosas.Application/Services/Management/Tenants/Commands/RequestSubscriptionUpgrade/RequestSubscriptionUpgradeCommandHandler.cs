using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionUpgrade;

public class RequestSubscriptionUpgradeCommandHandler : IRequestHandler<RequestSubscriptionUpgradeCommand, Result>
{
    #region Props 
    private readonly ILogger<RequestSubscriptionUpgradeCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionRenewalService _subscriptionRenewalService;
    #endregion



    #region Corts
    public RequestSubscriptionUpgradeCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionRenewalService subscriptionPlanChangingService,
                                                    ILogger<RequestSubscriptionUpgradeCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionRenewalService = subscriptionPlanChangingService;
        _logger = logger;
    }
    #endregion



    #region Handler   
    public async Task<Result> Handle(RequestSubscriptionUpgradeCommand command, CancellationToken cancellationToken)
    {
        return await _subscriptionRenewalService.EnableSubscriptionUpgradingAsync(command.SubscriptionId,
                                                                                     command.PlanId,
                                                                                     command.PlanPriceId,
                                                                                     command.CardReferenceId,
                                                                                     command.PaymentPlatform,
                                                                                     command.Comment, cancellationToken);
    }
    #endregion
}

