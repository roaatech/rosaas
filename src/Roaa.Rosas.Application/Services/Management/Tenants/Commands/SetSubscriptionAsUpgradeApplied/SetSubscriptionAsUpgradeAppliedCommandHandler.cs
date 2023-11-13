using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.SetSubscriptionAsUpgradeApplied;

public class SetSubscriptionAsUpgradeAppliedCommandHandler : IRequestHandler<SetSubscriptionAsUpgradeAppliedCommand, Result>
{
    #region Props 
    private readonly ILogger<SetSubscriptionAsUpgradeAppliedCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public SetSubscriptionAsUpgradeAppliedCommandHandler(IIdentityContextService identityContextService,
                                                    IRosasDbContext dbContext,
                                                    ILogger<SetSubscriptionAsUpgradeAppliedCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(SetSubscriptionAsUpgradeAppliedCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.Subscriptions
                                             .Where(x => x.ProductId == command.ProductId &&
                                                          command.TenantName.ToLower().Equals(x.Tenant.UniqueName))
                                             .SingleOrDefaultAsync(cancellationToken);
        if (subscription is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }

        if (subscription.SubscriptionPlanChangeStatus != SubscriptionPlanChangeStatus.InProgress || subscription.SubscriptionPlanChangeStatus != SubscriptionPlanChangeStatus.Failure)
        {
            return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
        }


        var date = DateTime.UtcNow;
        if (command.IsSuccessful)
        {
            subscription.SubscriptionPlanChangeStatus = SubscriptionPlanChangeStatus.Done;
            subscription.ModificationDate = DateTime.UtcNow;
            subscription.AddDomainEvent(new SubscriptionUpgradeAppliedDoneEvent(subscription));
        }
        else
        {
            subscription.SubscriptionPlanChangeStatus = SubscriptionPlanChangeStatus.Failure;
            subscription.AddDomainEvent(new SubscriptionUpgradeApplicationFailedEvent(subscription));
        }

        await _dbContext.SaveChangesAsync();

        return Result.Successful();
    }
    #endregion
}

