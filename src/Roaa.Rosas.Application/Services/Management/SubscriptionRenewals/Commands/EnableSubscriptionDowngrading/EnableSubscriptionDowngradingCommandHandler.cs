using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.EnableSubscriptionDowngrading;

public class EnableSubscriptionDowngradingCommandHandler : IRequestHandler<EnableSubscriptionDowngradingCommand, Result>
{
    #region Props 
    private readonly ILogger<EnableSubscriptionDowngradingCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionRenewalService _subscriptionRenewalService;
    #endregion



    #region Corts
    public EnableSubscriptionDowngradingCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionRenewalService subscriptionPlanChangingService,
                                                    ILogger<EnableSubscriptionDowngradingCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionRenewalService = subscriptionPlanChangingService;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(EnableSubscriptionDowngradingCommand command, CancellationToken cancellationToken)
    {
        return await _subscriptionRenewalService.EnableSubscriptionDowngradingAsync(command.SubscriptionId,
                                                                                        command.PlanId,
                                                                                        command.PlanPriceId,
                                                                                        command.CardReferenceId,
                                                                                        command.PaymentPlatform,
                                                                                        command.Comment, cancellationToken);
    }
    #endregion
}

