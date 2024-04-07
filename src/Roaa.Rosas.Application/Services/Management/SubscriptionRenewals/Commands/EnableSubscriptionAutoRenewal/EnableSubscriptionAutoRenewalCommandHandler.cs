using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.EnableSubscriptionAutoRenewal;

public class EnableSubscriptionAutoRenewalCommandHandler : IRequestHandler<EnableSubscriptionAutoRenewalCommand, Result>
{
    #region Props 
    private readonly ILogger<EnableSubscriptionAutoRenewalCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionRenewalService _subscriptionAutoRenewalService;
    #endregion



    #region Corts
    public EnableSubscriptionAutoRenewalCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionRenewalService subscriptionAutoRenewalService,
                                                    ILogger<EnableSubscriptionAutoRenewalCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionAutoRenewalService = subscriptionAutoRenewalService;
        _logger = logger;
    }
    #endregion



    #region Handler   
    public async Task<Result> Handle(EnableSubscriptionAutoRenewalCommand command, CancellationToken cancellationToken)
    {
        return await _subscriptionAutoRenewalService.EnableAutoRenewalAsync(command.SubscriptionId,
                                                                            command.CardReferenceId,
                                                                            command.PaymentPlatform,
                                                                            command.PlanPriceId,
                                                                            command.Comment,
                                                                            _identityContextService.UserId,
                                                                            _identityContextService.GetUserType(),
                                                                            command.RenewalsCount,
                                                                            command.IsContinuousRenewal,
                                                                            cancellationToken);
    }
    #endregion
}

