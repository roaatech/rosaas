﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionDowngrade;

public class RequestSubscriptionDowngradeCommandHandler : IRequestHandler<RequestSubscriptionDowngradeCommand, Result>
{
    #region Props 
    private readonly ILogger<RequestSubscriptionDowngradeCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public RequestSubscriptionDowngradeCommandHandler(IIdentityContextService identityContextService,
                                                    IRosasDbContext dbContext,
                                                    ILogger<RequestSubscriptionDowngradeCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(RequestSubscriptionDowngradeCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.Subscriptions
                                            .Where(x => _identityContextService.IsSuperAdmin() ||
                                                        _dbContext.EntityAdminPrivileges
                                                                .Any(a =>
                                                                    a.UserId == _identityContextService.UserId &&
                                                                    a.EntityId == x.TenantId &&
                                                                    a.EntityType == EntityType.Tenant
                                                                    )
                                                    )
                                            .Where(x => x.Id == command.SubscriptionId)
                                            .SingleOrDefaultAsync();

        if (subscription is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(command.SubscriptionId));
        }


        if (await _dbContext.SubscriptionPlanChanges
                                    .Where(x => x.Id == command.SubscriptionId)
                                    .AnyAsync())
        {
            return Result.Fail(ErrorMessage.SubscriptionAlreadyUpgradedDowngraded, _identityContextService.Locale, nameof(command.SubscriptionId));
        }


        var downgradeUrl = await _dbContext.Products.AsNoTracking()
                                             .Where(x => x.Id == subscription.ProductId)
                                             .Select(x => x.SubscriptionDowngradeUrl)
                                             .SingleOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(downgradeUrl))
        {
            return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
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
            Type = PlanChangingType.Downgrade,
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

        subscription.AddDomainEvent(new SubscriptionDowngradeRequestedEvent(subscription));

        _dbContext.SubscriptionPlanChanges.Add(subscriptionPlanChanging);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Successful();
    }
    #endregion
}

