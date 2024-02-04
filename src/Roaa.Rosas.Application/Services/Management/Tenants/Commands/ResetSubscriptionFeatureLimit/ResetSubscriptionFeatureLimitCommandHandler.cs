using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ResetSubscriptionFeatureLimit;

public class ResetSubscriptionFeatureLimitCommandHandler : IRequestHandler<ResetSubscriptionFeatureLimitCommand, Result>
{
    #region Props 
    private readonly ILogger<ResetSubscriptionFeatureLimitCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionService _subscriptionservice;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public ResetSubscriptionFeatureLimitCommandHandler(IIdentityContextService identityContextService,
                                            ISubscriptionService subscriptionservice,
                                            IRosasDbContext dbContext,
                                            ILogger<ResetSubscriptionFeatureLimitCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _subscriptionservice = subscriptionservice;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(ResetSubscriptionFeatureLimitCommand command, CancellationToken cancellationToken)
    {
        // Preparing to Retrieve Subscription's Features
        Expression<Func<SubscriptionFeature, bool>> predicate = x => x.Subscription.ProductId == command.ProductId &&
                                                                     x.Subscription.TenantId == command.TenantId;
        if (command.SubscriptionFeatureId is not null)
        {
            predicate = x => x.Subscription.ProductId == command.ProductId &&
                             x.Subscription.TenantId == command.TenantId &&
                             x.Id == command.SubscriptionFeatureId;
        }

        var subscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                   .Where(x => _identityContextService.IsSuperAdmin() ||
                                                                _dbContext.EntityAdminPrivileges
                                                                        .Any(a =>
                                                                            a.UserId == _identityContextService.UserId &&
                                                                            a.EntityId == x.Subscription.TenantId &&
                                                                            a.EntityType == EntityType.Tenant
                                                                            )
                                                          )
                                                   .Where(predicate)
                                                   .Include(x => x.Feature)
                                                   .ToListAsync();

        if (subscriptionFeatures is null || !subscriptionFeatures.Any())
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(command.TenantId));
        }

        var result = await _subscriptionservice.ResetSubscriptionsFeaturesAsync(subscriptionFeatures, command.Comment, null, cancellationToken);

        return result;
    }
    #endregion
}

