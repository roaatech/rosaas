using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
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
                                                   .Include(x => x.Feature)
                                                   .Where(predicate)
                                                   .ToListAsync();

        var result = await _subscriptionservice.ResetSubscriptionsFeaturesAsync(subscriptionFeatures, command.Comment, null, cancellationToken);

        return result;
    }
    #endregion
}

