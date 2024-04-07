using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.EnableSubscriptionUpgrading;

public class EnableSubscriptionUpgradingCommandHandler : IRequestHandler<EnableSubscriptionUpgradingCommand, Result>
{
    #region Props 
    private readonly ILogger<EnableSubscriptionUpgradingCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionRenewalService _subscriptionRenewalService;
    #endregion



    #region Corts
    public EnableSubscriptionUpgradingCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionRenewalService subscriptionPlanChangingService,
                                                    ILogger<EnableSubscriptionUpgradingCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionRenewalService = subscriptionPlanChangingService;
        _logger = logger;
    }
    #endregion



    #region Handler   
    public async Task<Result> Handle(EnableSubscriptionUpgradingCommand command, CancellationToken cancellationToken)
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

