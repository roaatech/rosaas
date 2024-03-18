using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.SubscriptionAutoRenewals;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CancelSubscriptionAutoRenewal;

public class CancelSubscriptionAutoRenewalCommandHandler : IRequestHandler<CancelSubscriptionAutoRenewalCommand, Result>
{
    #region Props 
    private readonly ILogger<CancelSubscriptionAutoRenewalCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionAutoRenewalService _subscriptionAutoRenewalService;
    #endregion



    #region Corts
    public CancelSubscriptionAutoRenewalCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionAutoRenewalService subscriptionAutoRenewalService,
                                                    ILogger<CancelSubscriptionAutoRenewalCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionAutoRenewalService = subscriptionAutoRenewalService;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(CancelSubscriptionAutoRenewalCommand command, CancellationToken cancellationToken)
    {
        return await _subscriptionAutoRenewalService.CancelAutoRenewalAsync(command.SubscriptionId,
                                                                            command.Comment,
                                                                            cancellationToken); ;

    }
    #endregion
}

