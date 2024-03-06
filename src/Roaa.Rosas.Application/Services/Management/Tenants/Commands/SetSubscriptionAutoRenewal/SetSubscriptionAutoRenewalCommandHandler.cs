using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.Subscriptions.AutoRenewals;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.SetSubscriptionAutoRenewal;

public class SetSubscriptionAutoRenewalCommandHandler : IRequestHandler<SetSubscriptionAutoRenewalCommand, Result>
{
    #region Props 
    private readonly ILogger<SetSubscriptionAutoRenewalCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionAutoRenewalService _subscriptionAutoRenewalService;
    #endregion



    #region Corts
    public SetSubscriptionAutoRenewalCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionAutoRenewalService subscriptionAutoRenewalService,
                                                    ILogger<SetSubscriptionAutoRenewalCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionAutoRenewalService = subscriptionAutoRenewalService;
        _logger = logger;
    }
    #endregion



    #region Handler   
    public async Task<Result> Handle(SetSubscriptionAutoRenewalCommand command, CancellationToken cancellationToken)
    {
        return await _subscriptionAutoRenewalService.EnableAutoRenewalAsync(command.SubscriptionId,
                                                                            command.CardReferenceId,
                                                                            command.PaymentPlatform,
                                                                            command.PlanPriceId,
                                                                            command.Comment,
                                                                            cancellationToken);
    }
    #endregion
}

