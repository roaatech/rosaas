using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionCycles
{
    public class GetSubscriptionCyclesQueryHandler : IRequestHandler<GetSubscriptionCyclesQuery, Result<List<SubscriptionCycleDto>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public GetSubscriptionCyclesQueryHandler(IRosasDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion


        #region Handler   
        public async Task<Result<List<SubscriptionCycleDto>>> Handle(GetSubscriptionCyclesQuery request, CancellationToken cancellationToken)
        {
            var subscriptionCycles = await _dbContext.SubscriptionCycles.AsNoTracking()
                                                 .Where(x => x.SubscriptionId == request.SubscriptionId &&
                                                                                (request.SubscriptionCycleId == null ||
                                                                                 request.SubscriptionCycleId == request.SubscriptionCycleId))
                                                             .Select(SubscriptionCycle => new SubscriptionCycleDto
                                                             {
                                                                 Id = SubscriptionCycle.Id,
                                                                 SubscriptionId = SubscriptionCycle.SubscriptionId,
                                                                 StartDate = SubscriptionCycle.StartDate,
                                                                 EndDate = SubscriptionCycle.EndDate,
                                                                 Cycle = SubscriptionCycle.Cycle,
                                                                 Price = SubscriptionCycle.Price,
                                                                 Plan = new CustomLookupItemDto<Guid>(SubscriptionCycle.PlanId, SubscriptionCycle.PlanDisplayName, SubscriptionCycle.PlanDisplayName),
                                                                 SubscriptionFeaturesCycles = SubscriptionCycle
                                                                                                 .SubscriptionFeaturesCycles
                                                                        .Select(featureCycle => new SubscriptionFeatureCycleDto
                                                                        {
                                                                            Id = featureCycle.Id,
                                                                            SubscriptionCycleId = featureCycle.SubscriptionCycleId,
                                                                            StartDate = featureCycle.StartDate,
                                                                            EndDate = featureCycle.EndDate,
                                                                            Feature = new CustomLookupItemDto<Guid>(featureCycle.FeatureId, featureCycle.FeatureDisplayName, featureCycle.FeatureDisplayName),
                                                                            Limit = featureCycle.Limit,
                                                                            Reset = featureCycle.FeatureReset,
                                                                            Type = featureCycle.FeatureType,
                                                                            Unit = featureCycle.FeatureUnit,
                                                                            RemainingUsage = featureCycle.RemainingUsage,
                                                                            TotalUsage = featureCycle.TotalUsage,
                                                                        }),
                                                             })
                                                             .ToListAsync(cancellationToken);

            return Result<List<SubscriptionCycleDto>>.Successful(subscriptionCycles);
        }
        #endregion
    }
}
