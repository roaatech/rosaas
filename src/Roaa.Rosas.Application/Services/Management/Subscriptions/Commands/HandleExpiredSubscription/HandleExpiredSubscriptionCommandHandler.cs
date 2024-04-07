using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals;
using Roaa.Rosas.Application.Services.Management.SubscriptionTrials;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.Commands.HandleExpiredSubscription;

public class HandleExpiredSubscriptionCommandHandler : IRequestHandler<HandleExpiredSubscriptionCommand, Result>
{
    #region Props 
    private readonly ILogger<HandleExpiredSubscriptionCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ISubscriptionRenewalService _subscriptionRenewalService;
    private readonly SubscriptionRenewalUtilities _subscriptionRenewalUtilities;
    private readonly TrialSubscriptionUtilities _trialSubscriptionUtilities;
    private readonly IRosasDbContext _dbContext;
    private DateTime _date;
    #endregion



    #region Corts
    public HandleExpiredSubscriptionCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionService subscriptionService,
                                                    ISubscriptionRenewalService subscriptionRenewalService,
                                                    IRosasDbContext dbContext,
                                                    SubscriptionRenewalUtilities subscriptionRenewalUtilities,
                                                    TrialSubscriptionUtilities trialSubscriptionUtilities,
                                                    ILogger<HandleExpiredSubscriptionCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionService = subscriptionService;
        _subscriptionRenewalService = subscriptionRenewalService;
        _dbContext = dbContext;
        _subscriptionRenewalUtilities = subscriptionRenewalUtilities;
        _trialSubscriptionUtilities = trialSubscriptionUtilities;
        _logger = logger;
    }
    #endregion



    #region Handler   
    public async Task<Result> Handle(HandleExpiredSubscriptionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _date = DateTime.UtcNow;
            var fromDate = _date;
            var toDate = fromDate.AddHours(command.RangeInHoursBetweenDates);

            var subscriptions = await _dbContext.Subscriptions
                                              .Include(x => x.SubscriptionRenewal)
                                              .Where(x => x.EndDate >= fromDate &&
                                                          x.IsActive &&
                                                          (
                                                            (
                                                                x.Trial == null &&
                                                                x.SubscriptionRenewal == null
                                                            ) ||
                                                            (
                                                                x.Trial != null &&
                                                                 _trialSubscriptionUtilities.AllowedTrialSubscriptionStatusesForForcedDowngrade
                                                                                            .Contains(x.Trial.Status)
                                                            ) ||
                                                            (
                                                                x.SubscriptionRenewal != null &&
                                                                _subscriptionRenewalUtilities.AllowedSubscriptionRenewalStatusForForcedDowngrade
                                                                                             .Contains(x.SubscriptionRenewal.Status)
                                                            )
                                                         ))
                                              .ToListAsync(cancellationToken);

            if (!subscriptions.Any()) return Result.Successful();

            var plans = await _dbContext.Plans
                               .AsNoTracking()
                               .Where(x => x.AlternativePlanId != null &&
                                           subscriptions.Select(s => s.PlanId).Distinct().Contains(x.Id))
                               .Select(x => new
                               {
                                   x.Id,
                                   x.AlternativePlanId,
                                   x.AlternativePlanPriceId,
                                   x.TrialPeriodInDays
                               })
                               .ToListAsync(cancellationToken);

            foreach (var subscription in subscriptions)
            {
                // # case 1
                if (subscription.SubscriptionRenewal != null &&
                    _subscriptionRenewalUtilities.EnsureIsForcedDowngrade(subscription.SubscriptionRenewal))
                {
                    await _subscriptionService.SuspendSubscriptionAsync(subscription, cancellationToken);
                }


                // # case 2
                var plan = plans.Where(x => x.Id == subscription.PlanId).SingleOrDefault();
                if (plan is not null)
                {
                    await _subscriptionRenewalService.EnableSubscriptionDowngradingAsync(subscription,
                                                                                         plan.AlternativePlanId.Value,
                                                                                         plan.AlternativePlanPriceId.Value,
                                                                                         "The subscription will be forcibly downgraded due to the subscription period expiring.",
                                                                                         cancellationToken);
                    continue;
                }

                // # case 3
                await _subscriptionService.SuspendSubscriptionAsync(subscription, cancellationToken);
            }
            return Result.Successful();
        }
        catch (Exception ex)
        {
            var errorMsg = $"An error occurred while executing the {this.GetType().Name}!";
            _logger.LogError(ex, errorMsg);
            return Result.Fail(errorMsg);
        }
    }


    #endregion


}

