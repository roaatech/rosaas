using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.SubscriptionPlansChanging;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionDowngrade;

public class RequestSubscriptionDowngradeCommandHandler : IRequestHandler<RequestSubscriptionDowngradeCommand, Result>
{
    #region Props 
    private readonly ILogger<RequestSubscriptionDowngradeCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionPlanChangingService _subscriptionPlanChangingService;
    #endregion



    #region Corts
    public RequestSubscriptionDowngradeCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionPlanChangingService subscriptionPlanChangingService,
                                                    ILogger<RequestSubscriptionDowngradeCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionPlanChangingService = subscriptionPlanChangingService;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(RequestSubscriptionDowngradeCommand command, CancellationToken cancellationToken)
    {
        return await _subscriptionPlanChangingService.CreateSubscriptionDowngradeAsync(command.SubscriptionId,
                                                                                        command.PlanId,
                                                                                        command.PlanPriceId,
                                                                                        command.CardReferenceId,
                                                                                        command.PaymentPlatform,
                                                                                        command.Comment, cancellationToken);
    }
    #endregion
}

