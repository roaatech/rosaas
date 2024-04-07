using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.EventHandlers;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.ApplyUpgradeToSubscription;

public class ApplyUpgradeToSubscriptionCommandHandler : IRequestHandler<ApplyUpgradeToSubscriptionCommand, Result>
{
    #region Props 
    private readonly ILogger<ApplyUpgradeToSubscriptionCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly IRosasDbContext _dbContext;
    private readonly ISubscriptionService _subscriptionService;
    private readonly SubscriptionUpgradingProcessor _subscriptionUpgradingProcessor;
    #endregion



    #region Corts
    public ApplyUpgradeToSubscriptionCommandHandler(IIdentityContextService identityContextService,
                                                    IRosasDbContext dbContext,
                                                    ISubscriptionService subscriptionService,
                                                    SubscriptionUpgradingProcessor subscriptionUpgradingProcessor,
                                                    ILogger<ApplyUpgradeToSubscriptionCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _dbContext = dbContext;
        _subscriptionService = subscriptionService;
        _subscriptionUpgradingProcessor = subscriptionUpgradingProcessor;
        _logger = logger;
    }
    #endregion



    #region Handler   
    public async Task<Result> Handle(ApplyUpgradeToSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.Subscriptions
                                            .Include(x => x.SubscriptionRenewal)
                                             .Where(x => x.ProductId == command.ProductId &&
                                                          command.TenantName.ToLower().Equals(x.Tenant.SystemName) &&
                                                           x.SubscriptionRenewal != null &&
                                                            x.SubscriptionRenewal.Type == SubscriptionRenewalTypeEnum.Upgrade)
                                             .SingleOrDefaultAsync(cancellationToken);
        if (subscription is null || subscription.SubscriptionRenewal is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }

        if (subscription.SubscriptionRenewal.Status != SubscriptionRenewalStatus.PendingExternalSystem)
        {
            return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
        }

        var date = DateTime.UtcNow;
        if (command.IsSuccessful)
        {
            await _subscriptionUpgradingProcessor.ApplyUpgradeToSubscriptionAsync(subscription, subscription.SubscriptionRenewal, cancellationToken);
        }
        else
        {
            subscription.SubscriptionRenewal.Status = SubscriptionRenewalStatus.FailedExternalSystem;
            subscription.SubscriptionRenewal.ModificationDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }


        return Result.Successful();
    }
    #endregion
}

