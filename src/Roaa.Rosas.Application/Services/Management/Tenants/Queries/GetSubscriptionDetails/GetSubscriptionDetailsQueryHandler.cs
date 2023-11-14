using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

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
                                                     SubscriptionResetStatus = subscription.SubscriptionResetStatus,
                                                     SubscriptionPlanChangeStatus = subscription.SubscriptionPlanChangeStatus,
                                                     IsSubscriptionResetUrlExists = !string.IsNullOrWhiteSpace(subscription.Product.SubscriptionResetUrl),
                                                     IsSubscriptionUpgradeUrlExists = !string.IsNullOrWhiteSpace(subscription.Product.SubscriptionUpgradeUrl),
                                                     IsSubscriptionDowngradeUrlExists = !string.IsNullOrWhiteSpace(subscription.Product.SubscriptionDowngradeUrl),
                                                     Plan = new CustomLookupItemDto<Guid>
                                                     {
                                                         Id = subscription.Plan.Id,
                                                         Name = subscription.Plan.Name,
                                                         Title = subscription.Plan.DisplayName,
                                                     },
                                                     PlanPrice = new PlanPriceDto
                                                     {
                                                         Id = subscription.PlanPrice.Id,
                                                         Cycle = subscription.PlanPrice.PlanCycle,
                                                         Price = subscription.PlanPrice.Price,
                                                     },
                                                     SubscriptionCycles = subscription.SubscriptionCycles.Select(SubscriptionCycle => new SubscriptionCycleDto
                                                     {
                                                         Id = SubscriptionCycle.Id,
                                                         StartDate = SubscriptionCycle.StartDate,
                                                         EndDate = SubscriptionCycle.EndDate,
                                                         Cycle = SubscriptionCycle.Cycle,
                                                         Price = SubscriptionCycle.Price,
                                                         Plan = new CustomLookupItemDto<Guid>(SubscriptionCycle.PlanId, SubscriptionCycle.PlanDisplayName, SubscriptionCycle.PlanDisplayName),

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
                                                             Title = subscriptionFeature.Feature.DisplayName,
                                                             Description = subscriptionFeature.Feature.Description,
                                                             Type = subscriptionFeature.Feature.Type,
                                                             Reset = subscriptionFeature.Feature.Reset,
                                                             Limit = subscriptionFeature.PlanFeature.Limit,
                                                             Unit = subscriptionFeature.PlanFeature.FeatureUnit,
                                                         },
                                                         Type = subscriptionFeature.Feature.Type,
                                                         Reset = subscriptionFeature.Feature.Reset,
                                                         Limit = subscriptionFeature.PlanFeature.Limit,
                                                         Unit = subscriptionFeature.PlanFeature.FeatureUnit,
                                                         SubscriptionFeaturesCycles = subscriptionFeature.SubscriptionFeatureCycles.Select(featureCycle => new SubscriptionFeatureCycleDto
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
                                                     }),
                                                     AutoRenewal = subscription.AutoRenewal == null ? null : new SubscriptionAutoRenewalDto
                                                     {
                                                         Cycle = subscription.AutoRenewal.PlanCycle,
                                                         Price = subscription.AutoRenewal.Price,
                                                         EditedDate = subscription.AutoRenewal.ModificationDate,
                                                         CreatedDate = subscription.AutoRenewal.CreationDate,
                                                         Comment = subscription.AutoRenewal.Comment,
                                                     },
                                                     SubscriptionPlanChange = subscription.SubscriptionPlanChanging == null ? null : new SubscriptionPlanChangingDto
                                                     {
                                                         PlanDisplayName = subscription.SubscriptionPlanChanging.PlanDisplayName,
                                                         Type = subscription.SubscriptionPlanChanging.Type,
                                                         Cycle = subscription.SubscriptionPlanChanging.PlanCycle,
                                                         Price = subscription.SubscriptionPlanChanging.Price,
                                                         EditedDate = subscription.SubscriptionPlanChanging.ModificationDate,
                                                         CreatedDate = subscription.SubscriptionPlanChanging.CreationDate,
                                                         Comment = subscription.SubscriptionPlanChanging.Comment,
                                                     },
                                                 })
                                                 .SingleOrDefaultAsync(cancellationToken);


            if (subscription is not null)
            {
                subscription.HasSubscriptionFeaturesLimitsResettable = subscription.SubscriptionFeatures
                                                                                    .Select(x => x.Feature.Reset)
                                                                                    .Where(reset => FeatureResetManager.FromKey(reset).IsResettable())
                                                                                    .Any();

                subscription.IsPlanChangeAllowed = subscription.SubscriptionPlanChange is null &&
                                                    (subscription.SubscriptionPlanChangeStatus is null ||
                                                     subscription.SubscriptionPlanChangeStatus == SubscriptionPlanChangeStatus.Done) &&
                                                     subscription.IsSubscriptionUpgradeUrlExists &&
                                                     subscription.IsSubscriptionDowngradeUrlExists;

                subscription.IsResettableAllowed = (subscription.LastResetDate is null || DateTime.UtcNow > subscription.LastResetDate.Value.AddHours(24)) &&
                                                   (subscription.SubscriptionResetStatus is null ||
                                                    subscription.SubscriptionResetStatus == SubscriptionResetStatus.Done) &&
                                                    subscription.IsSubscriptionResetUrlExists;

            }

            return Result<SubscriptionDetailsDto>.Successful(subscription);
        }
        #endregion
    }
}
