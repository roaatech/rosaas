using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
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
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly SubscriptionRenewalUtilities _subscriptionRenewalUtilities;
    private readonly TrialSubscriptionUtilities _trialSubscriptionUtilities;
    private readonly IRosasDbContext _dbContext;
    private DateTime _date;
    #endregion



    #region Corts
    public HandleExpiredSubscriptionCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionService subscriptionService,
                                                    ISubscriptionRenewalService subscriptionRenewalService,
                                                    IGenericAttributeService genericAttributeService,
                                                    IRosasDbContext dbContext,
                                                    SubscriptionRenewalUtilities subscriptionRenewalUtilities,
                                                    TrialSubscriptionUtilities trialSubscriptionUtilities,
                                                    ILogger<HandleExpiredSubscriptionCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionService = subscriptionService;
        _subscriptionRenewalService = subscriptionRenewalService;
        _genericAttributeService = genericAttributeService;
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
            var toDate = _date;
            var fromDate = toDate.AddHours(-command.RangeInHoursBetweenDates);

            var subscriptions = await _dbContext.Subscriptions
                                                   .Where(x => fromDate <= x.EndDate && x.EndDate <= toDate &&
                                                               x.IsActive
                                                               &&
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
                try
                {




                    var plan = plans.Where(x => x.Id == subscription.PlanId).SingleOrDefault();
                    if (plan is not null)
                    {
                        // # case 1  
                        var result = await _subscriptionRenewalService.TryToEnableForcedDowngradeAsync(subscription,
                                                                                     plan.AlternativePlanId!.Value,
                                                                                     plan.AlternativePlanPriceId!.Value,
                                                                                     "The subscription will be forcibly downgraded due to the subscription period expiring.",
                                                                                     cancellationToken);

                        //In case the forced downgrade fails to enable, that means the enabling forced-downgrade has failed,
                        //or the subscription already has a forced-downgrade and fails to apply it,
                        //so the system should suspend the subscription.
                        if (result.Success) continue;
                    }

                    // # case 2  
                    await _subscriptionService.SuspendSubscriptionAsync(subscription, cancellationToken);
                }
                catch (Exception ex)
                {
                    var errorMsg = $"An error occurred in handler({this.GetType().Name}) while handling the subscription, with identifier id:{subscription.Id}!";
                    _logger.LogError(ex, errorMsg);
                }
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
    /*
     

            var trialSubscriptions = await _dbContext.TrialSubscriptions
                                                     .AsNoTracking()
                                                     .Where(x => subscriptionsIds.Contains(x.SubscriptionId))
                                                     .ToListAsync(cancellationToken);

            var subscriptionRenewals = await _dbContext.SubscriptionRenewals
                                                     .AsNoTracking()
                                                     .Where(x => subscriptionsIds.Contains(x.SubscriptionId))
                                                     .ToListAsync(cancellationToken);


            subscriptionsIds = subscriptionsIds.Where(subscriptionId => (
                                                          !trialSubscriptions.Select(x => x.SubscriptionId).Contains(subscriptionId) &&
                                                          !subscriptionRenewals.Select(x => x.SubscriptionId).Contains(subscriptionId)
                                                      )
                                                      ||
                                                      (
                                                          trialSubscriptions.Select(x => x.SubscriptionId).Contains(subscriptionId) &&
                                                          _trialSubscriptionUtilities.AllowedTrialSubscriptionStatusesForForcedDowngrade
                                                          .Contains(trialSubscriptions.Where(x => x.SubscriptionId == subscriptionId).FirstOrDefault()!.Status)
                                                      )
                                                      ||
                                                      (
                                                          subscriptionRenewals.Select(x => x.SubscriptionId).Contains(subscriptionId) &&
                                                          _subscriptionRenewalUtilities.AllowedSubscriptionRenewalStatusForForcedDowngrade
                                                          .Contains(subscriptionRenewals.Where(x => x.SubscriptionId == subscriptionId).FirstOrDefault()!.Status)
                                                      ))
                                                      .ToList();

            var subscriptions = await _dbContext.Subscriptions
                                                .Where(x => subscriptionsIds.Contains(x.Id))
                                                .ToListAsync(cancellationToken);


     */

}

