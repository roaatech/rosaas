using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionUpgrade;

public class RequestSubscriptionUpgradeCommandHandler : IRequestHandler<RequestSubscriptionUpgradeCommand, Result>
{
    #region Props 
    private readonly ILogger<RequestSubscriptionUpgradeCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public RequestSubscriptionUpgradeCommandHandler(IIdentityContextService identityContextService,
                                                    IRosasDbContext dbContext,
                                                    ILogger<RequestSubscriptionUpgradeCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(RequestSubscriptionUpgradeCommand command, CancellationToken cancellationToken)
    {
        if (await _dbContext.SubscriptionPlanChanges
                                    .Where(x => x.Id == command.SubscriptionId)
                                    .AnyAsync())
        {
            return Result.Fail(ErrorMessage.SubscriptionAlreadyUpgradedDowngraded, _identityContextService.Locale, nameof(command.SubscriptionId));
        }

        var subscription = await _dbContext.Subscriptions
                                  .Where(x => x.Id == command.SubscriptionId)
                                  .SingleOrDefaultAsync();

        if (subscription is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(command.SubscriptionId));
        }

        var plan = await _dbContext.Plans
                                    .Where(x => x.Id == command.PlanId)
                                    .SingleOrDefaultAsync();
        if (plan is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(command.PlanId));
        }

        var planPrice = await _dbContext.PlanPrices
                                        .Where(x => x.Id == command.PlanPriceId &&
                                                    x.PlanId == command.PlanId)

                                        .SingleOrDefaultAsync();
        if (planPrice is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(command.PlanPriceId));
        }

        if (subscription.ProductId != plan.ProductId)
        {
            return Result.Fail(ErrorMessage.PlanDoesNotBelongToProduct, _identityContextService.Locale, nameof(command.PlanId));
        }

        var date = DateTime.UtcNow;

        var subscriptionPlanChanging = new SubscriptionPlanChanging
        {
            Id = command.SubscriptionId,
            Type = PlanChangingType.Upgrade,
            SubscriptionId = command.SubscriptionId,
            PlanPriceId = command.PlanPriceId,
            PlanId = planPrice.PlanId,
            PlanCycle = planPrice.PlanCycle,
            Price = planPrice.Price,
            PlanDisplayName = plan.DisplayName ?? "",
            IsPaid = true,
            Comment = command.Comment,
            CreatedByUserId = _identityContextService.UserId,
            ModifiedByUserId = _identityContextService.UserId,
            CreationDate = date,
            ModificationDate = date,
        };

        subscription.SubscriptionPlanChangeStatus = null;

        subscription.AddDomainEvent(new SubscriptionUpgradeRequestedEvent(subscription));

        _dbContext.SubscriptionPlanChanges.Add(subscriptionPlanChanging);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Successful();
    }
    #endregion
}

