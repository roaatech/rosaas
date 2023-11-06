using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
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
    private readonly IExternalSystemAPI _externalSystemAPI;
    private readonly IProductService _productService;
    private readonly ISubscriptionService _subscriptionservice;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public ResetSubscriptionFeatureLimitCommandHandler(IIdentityContextService identityContextService,
                                            IExternalSystemAPI externalSystemAPI,
                                            IProductService productService,
                                            ISubscriptionService subscriptionservice,
                                            IRosasDbContext dbContext,
                                            ILogger<ResetSubscriptionFeatureLimitCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _externalSystemAPI = externalSystemAPI;
        _productService = productService;
        _subscriptionservice = subscriptionservice;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(ResetSubscriptionFeatureLimitCommand request, CancellationToken cancellationToken)
    {
        // Preparing to Retrieve Subscription's Features
        Expression<Func<SubscriptionFeature, bool>> predicate = x => x.Subscription.ProductId == request.ProductId &&
                                                                     x.Subscription.TenantId == request.TenantId;
        if (request.SubscriptionFeatureId is not null)
        {
            predicate = x => x.Subscription.ProductId == request.ProductId &&
                             x.Subscription.TenantId == request.TenantId &&
                             x.Id == request.SubscriptionFeatureId;
        }


        var subscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                   .Include(x => x.Feature)
                                                   .Where(predicate)
                                                   .ToListAsync();

        var result = await _subscriptionservice.ResetSubscriptionsFeaturesAsync(subscriptionFeatures, request.Comment, null, cancellationToken);

        return result;
    }
    #endregion
}

