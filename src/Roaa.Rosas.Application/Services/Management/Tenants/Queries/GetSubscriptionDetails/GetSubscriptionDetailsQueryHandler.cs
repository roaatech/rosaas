using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionDetails
{
    public class GetSubscriptionDetailsQueryHandler : IRequestHandler<GetSubscriptionDetailsQuery, Result<SubscriptionDetailsDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public GetSubscriptionDetailsQueryHandler(IRosasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion


        #region Handler   
        public async Task<Result<SubscriptionDetailsDto>> Handle(GetSubscriptionDetailsQuery request, CancellationToken cancellationToken)
        {
            var subscription = await _dbContext.Subscriptions.AsNoTracking()
                                                 .Where(x => x.TenantId == request.TenantId &&
                                                             x.ProductId == request.ProductId)
                                                 .Select(subscription => new SubscriptionDetailsDto
                                                 {
                                                     SubscriptionId = subscription.Id,
                                                     CurrentSubscriptionCycleId = subscription.SubscriptionCycleId,
                                                     StartDate = subscription.StartDate,
                                                     EndDate = subscription.EndDate,
                                                     LastResetDate = subscription.LastResetDate,
                                                     LastLimitsResetDate = subscription.LastLimitsResetDate,
                                                     Plan = new LookupItemDto<Guid>
                                                     {
                                                         Id = subscription.Plan.Id,
                                                         Name = subscription.Plan.Name,
                                                     },
                                                     PlanPrice = new PlanPriceDto
                                                     {
                                                         Id = subscription.PlanPrice.Id,
                                                         Cycle = subscription.PlanPrice.Cycle,
                                                         Price = subscription.PlanPrice.Price,
                                                     },
                                                     SubscriptionCycles = subscription.SubscriptionCycles.Select(SubscriptionCycle => new SubscriptionCycleDto
                                                     {
                                                         Id = SubscriptionCycle.Id,
                                                         StartDate = SubscriptionCycle.StartDate,
                                                         EndDate = SubscriptionCycle.EndDate,
                                                         Cycle = SubscriptionCycle.Cycle,
                                                         Price = SubscriptionCycle.Price,
                                                         Plan = new LookupItemDto<Guid>(SubscriptionCycle.PlanId, SubscriptionCycle.PlanName),

                                                     }),
                                                     SubscriptionFeatures = subscription.SubscriptionFeatures.Select(subscriptionFeature => new SubscriptionFeatureDto
                                                     {
                                                         Id = subscriptionFeature.Id,
                                                         CurrentSubscriptionFeatureCycleId = subscriptionFeature.SubscriptionFeatureCycleId,
                                                         EndDate = subscriptionFeature.EndDate,
                                                         StartDate = subscriptionFeature.StartDate,
                                                         RemainingUsage = subscriptionFeature.RemainingUsage,
                                                         Feature = new FeatureDto
                                                         {
                                                             Id = subscriptionFeature.Feature.Id,
                                                             Name = subscriptionFeature.Feature.Name,
                                                             Description = subscriptionFeature.Feature.Description,
                                                             Type = subscriptionFeature.Feature.Type,
                                                             Reset = subscriptionFeature.Feature.Reset,
                                                             Limit = subscriptionFeature.PlanFeature.Limit,
                                                             Unit = subscriptionFeature.PlanFeature.Unit,
                                                         },
                                                         Type = subscriptionFeature.Feature.Type,
                                                         Reset = subscriptionFeature.Feature.Reset,
                                                         Limit = subscriptionFeature.PlanFeature.Limit,
                                                         Unit = subscriptionFeature.PlanFeature.Unit,
                                                         SubscriptionFeaturesCycles = subscriptionFeature.SubscriptionFeatureCycles.Select(featureCycle => new SubscriptionFeatureCycleDto
                                                         {
                                                             Id = featureCycle.Id,
                                                             SubscriptionCycleId = featureCycle.SubscriptionCycleId,
                                                             StartDate = featureCycle.StartDate,
                                                             EndDate = featureCycle.EndDate,
                                                             Feature = new LookupItemDto<Guid>(featureCycle.FeatureId, featureCycle.FeatureName),
                                                             Limit = featureCycle.Limit,
                                                             Reset = featureCycle.Reset,
                                                             Type = featureCycle.Type,
                                                             Unit = featureCycle.Unit,
                                                             RemainingUsage = featureCycle.RemainingUsage,
                                                             TotalUsage = featureCycle.TotalUsage,
                                                         }),
                                                     }),
                                                     AutoRenewal = subscription.AutoRenewal == null ? null : new SubscriptionAutoRenewalDto
                                                     {
                                                         Cycle = subscription.AutoRenewal.Cycle,
                                                         Price = subscription.AutoRenewal.Price,
                                                         EditedDate = subscription.AutoRenewal.ModificationDate,
                                                         CreatedDate = subscription.AutoRenewal.CreationDate,
                                                     },
                                                 })
                                                 .SingleOrDefaultAsync(cancellationToken);

            return Result<SubscriptionDetailsDto>.Successful(subscription);
        }
        #endregion
    }
}
