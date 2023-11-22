using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ResetSubscription;

public class ResetSubscriptionCommandHandler : IRequestHandler<ResetSubscriptionCommand, Result>
{
    #region Props 
    private readonly ILogger<ResetSubscriptionCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public ResetSubscriptionCommandHandler(IIdentityContextService identityContextService,
                                            IRosasDbContext dbContext,
                                            ILogger<ResetSubscriptionCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(ResetSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.Subscriptions
                                           .Where(x => x.ProductId == command.ProductId &&
                                                        command.TenantName.ToLower().Equals(x.Tenant.UniqueName))
                                           .SingleOrDefaultAsync(cancellationToken);
        if (subscription is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }


        var date = DateTime.UtcNow;
        if (command.IsSuccessful)
        {
            subscription.LastResetDate = date;
            subscription.SubscriptionResetStatus = SubscriptionResetStatus.Done;
            subscription.AddDomainEvent(new SubscriptionReseAppliedDoneEvent(subscription));
        }
        else
        {
            subscription.ResetOperationDate = date;
            subscription.SubscriptionResetStatus = SubscriptionResetStatus.Failure;
            subscription.AddDomainEvent(new SubscriptionResetApplicationFailedEvent(subscription));
        }

        await _dbContext.SaveChangesAsync();

        return Result.Successful();


    }
    #endregion
}

