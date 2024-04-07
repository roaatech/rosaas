using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.CancelSubscriptionRenewal;

public class CancelSubscriptionRenewalCommandHandler : IRequestHandler<CancelSubscriptionRenewalCommand, Result>
{
    #region Props 
    private readonly ILogger<CancelSubscriptionRenewalCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionRenewalService _subscriptionRenewalService;
    #endregion



    #region Corts
    public CancelSubscriptionRenewalCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionRenewalService subscriptionAutoRenewalService,
                                                    ILogger<CancelSubscriptionRenewalCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionRenewalService = subscriptionAutoRenewalService;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(CancelSubscriptionRenewalCommand command, CancellationToken cancellationToken)
    {
        return await _subscriptionRenewalService.CancelRenewalAsync(command.SubscriptionRenewalId,
                                                                            command.SubscriptionId,
                                                                            command.Comment,
                                                                            cancellationToken); ;

    }
    #endregion
}

