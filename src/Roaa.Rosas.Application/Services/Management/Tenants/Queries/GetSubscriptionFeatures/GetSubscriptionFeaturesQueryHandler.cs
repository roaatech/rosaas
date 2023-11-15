using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionFeatures
{
    public class GetSubscriptionFeaturesQueryHandler : IRequestHandler<GetSubscriptionFeaturesQuery, Result<List<SubscriptionFeatureDto>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public GetSubscriptionFeaturesQueryHandler(IRosasDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion


        #region Handler   
        public async Task<Result<List<SubscriptionFeatureDto>>> Handle(GetSubscriptionFeaturesQuery request, CancellationToken cancellationToken)
        {
            var subscriptionFeatures = await _dbContext.SubscriptionFeatures.AsNoTracking()
                                                 .Where(x => x.SubscriptionId == request.SubscriptionId)
                                                             .Select(subscriptionFeature => new SubscriptionFeatureDto
                                                             {
                                                                 Id = subscriptionFeature.Id,
                                                                 CurrentSubscriptionFeatureCycleId = subscriptionFeature.SubscriptionFeatureCycleId,
                                                                 EndDate = subscriptionFeature.EndDate,
                                                                 StartDate = subscriptionFeature.StartDate,
                                                                 RemainingUsage = subscriptionFeature.RemainingUsage,
                                                                 Type = subscriptionFeature.Feature.Type,
                                                                 Reset = subscriptionFeature.Feature.FeatureReset,
                                                                 Limit = subscriptionFeature.PlanFeature.Limit,
                                                                 Unit = subscriptionFeature.PlanFeature.FeatureUnit,
                                                                 Feature = new LookupItemDto<Guid>
                                                                 {
                                                                     Id = subscriptionFeature.Feature.Id,
                                                                     Name = subscriptionFeature.Feature.DisplayName,
                                                                 },
                                                             })
                                                             .ToListAsync(cancellationToken);

            return Result<List<SubscriptionFeatureDto>>.Successful(subscriptionFeatures);
        }
        #endregion
    }
}
