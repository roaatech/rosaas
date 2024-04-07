using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionTrials.Commands.UpgradeTrialSubscriptionToStandard;

public class UpgradeTrialSubscriptionToStandardCommandHandler : IRequestHandler<UpgradeTrialSubscriptionToStandardCommand, Result>
{
    #region Props 
    private readonly ILogger<UpgradeTrialSubscriptionToStandardCommandHandler> _logger;
    private readonly TrialSubscriptionUtilities _utilities;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPaymentService _paymentService;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public UpgradeTrialSubscriptionToStandardCommandHandler(IIdentityContextService identityContextService,
                                                    ISubscriptionService subscriptionService,
                                                    IPaymentService paymentService,
                                                    IRosasDbContext dbContext,
                                                    TrialSubscriptionUtilities utilities,
                                                    ILogger<UpgradeTrialSubscriptionToStandardCommandHandler> logger)
    {
        _utilities = utilities;
        _identityContextService = identityContextService;
        _subscriptionService = subscriptionService;
        _paymentService = paymentService;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion



    #region  
    public async Task<Result> Handle(UpgradeTrialSubscriptionToStandardCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var fromDate = DateTime.UtcNow;
            var toDate = fromDate.AddHours(command.RangeInHoursBetweenDates);

            var trialSubscriptions = await _dbContext.TrialSubscriptions
                                                     .Include(x => x.Subscription)
                                                     .Where(x => x.EndDate >= fromDate && x.EndDate <= toDate)
                                                     .ToListAsync(cancellationToken);

            foreach (var trial in trialSubscriptions)
            {
                ArgumentNullException.ThrowIfNull(trial.Subscription);

                if (!_utilities.EnsurethatTheTrialSubscriptionStatusIsAllowedForUpgrade(trial.Status))
                {
                    throw new NullReferenceException($"Cannot handle({trial.GetType().Name}) in {trial.Status} status.");
                }

                var orderId = await _dbContext.Tenants
                                                   .Where(x => x.Id == trial.Subscription.TenantId)
                                                   .Select(x => x.LastOrderId)
                                                   .SingleAsync(cancellationToken);

                await UpdateSubscriptionTrialStatusAsync(trial, SubscriptionTrialStatus.PendingPayment, cancellationToken);

                var paymentResult = await _paymentService.CapturePaymentAsync(orderId, PaymentPurpose.UpgradeTrialSubscriptionToStandard, cancellationToken);
                if (!paymentResult.Success)
                {
                    await UpdateSubscriptionTrialStatusAsync(trial, SubscriptionTrialStatus.FailedPayment, cancellationToken);
                    continue;
                }

                await UpdateSubscriptionTrialStatusAsync(trial, SubscriptionTrialStatus.Processing, cancellationToken);

                var resetResult = await _subscriptionService.ResetSubscriptionPlanAsync(trial.Subscription,
                                                                                        trial.SelectedPlanId,
                                                                                        trial.SelectedPlanPriceId);
                if (!resetResult.Success)
                {
                    await UpdateSubscriptionTrialStatusAsync(trial, SubscriptionTrialStatus.Failure, cancellationToken);
                    continue;
                }

                trial.Subscription.SubscriptionMode = SubscriptionMode.Standard;
                trial.Subscription.ModificationDate = DateTime.UtcNow;

                _dbContext.TrialSubscriptions.Remove(trial);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await _subscriptionService.ActivateSubscriptionAsync(trial.Subscription, cancellationToken);
            }
            return Result.Successful();

        }

        catch (Exception ex)
        {
            var errorMsg = $"An error occurred while executing the Handle() function of {this.GetType().Name}!";
            _logger.LogError(ex, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
    #endregion


    private async Task UpdateSubscriptionTrialStatusAsync(TrialSubscription trial, SubscriptionTrialStatus status, CancellationToken cancellationToken = default)
    {
        trial.Status = status;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

