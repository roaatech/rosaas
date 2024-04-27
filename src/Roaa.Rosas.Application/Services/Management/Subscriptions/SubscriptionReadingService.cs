using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public partial class SubscriptionService : ISubscriptionService
    {

        #region Services 
        public async Task<Result<List<SubscriptionFeatureDto>>> GetSubscriptionFeaturesAsync(Guid subscriptionId, CancellationToken cancellationToken)
        {
            var subscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                    .AsNoTracking()
                                                    .Where(x => _identityContextService.IsSuperAdmin() ||
                                                            _dbContext.EntityAdminPrivileges
                                                                        .Any(a =>
                                                                            a.UserId == _identityContextService.UserId &&
                                                                            a.EntityId == x.Subscription.TenantId &&
                                                                            a.EntityType == EntityType.Tenant
                                                                            )
                                                        )
                                                    .Where(x => x.SubscriptionId == subscriptionId)
                                                             .Select(subscriptionFeature => new SubscriptionFeatureDto
                                                             {
                                                                 Id = subscriptionFeature.Id,
                                                                 CurrentSubscriptionFeatureCycleId = subscriptionFeature.SubscriptionFeatureCycleId,
                                                                 EndDate = subscriptionFeature.EndDate,
                                                                 StartDate = subscriptionFeature.StartDate,
                                                                 RemainingUsage = subscriptionFeature.RemainingUsage,
                                                                 Type = subscriptionFeature.Feature!.Type,
                                                                 Reset = subscriptionFeature.PlanFeature!.FeatureReset,
                                                                 Limit = subscriptionFeature.PlanFeature.Limit,
                                                                 Unit = subscriptionFeature.PlanFeature.FeatureUnit,
                                                                 UnitDisplayName = subscriptionFeature.PlanFeature.UnitDisplayName,
                                                                 Feature = new LookupItemDto<Guid>
                                                                 {
                                                                     Id = subscriptionFeature.Feature.Id,
                                                                     SystemName = subscriptionFeature.Feature.DisplayName,
                                                                 },
                                                             })
                                                             .ToListAsync(cancellationToken);

            return Result<List<SubscriptionFeatureDto>>.Successful(subscriptionFeatures);
        }

        public async Task<Result<SubscriptionDetailsDto>> GetSubscriptionDetailsAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken)
        {

            var subscription = await _dbContext.Subscriptions
                                                .AsNoTracking()
                                                .Where(x => _identityContextService.IsSuperAdmin() ||
                                                            _dbContext.EntityAdminPrivileges
                                                                        .Any(a =>
                                                                            a.UserId == _identityContextService.UserId &&
                                                                            a.EntityId == x.TenantId &&
                                                                            a.EntityType == EntityType.Tenant
                                                                            )
                                                        )
                                                .Where(x => x.TenantId == tenantId &&
                                                             x.ProductId == productId)
                                                 .Select(subscription => new SubscriptionDetailsDto
                                                 {
                                                     SubscriptionId = subscription.Id,
                                                     SubscriptionMode = subscription.SubscriptionMode,
                                                     CurrentSubscriptionCycleId = subscription.SubscriptionCycleId,
                                                     StartDate = subscription.StartDate,
                                                     EndDate = subscription.EndDate,
                                                     LastResetDate = subscription.LastResetDate,
                                                     LastLimitsResetDate = subscription.LastLimitsResetDate,
                                                     SubscriptionResetStatus = subscription.SubscriptionResetStatus,
                                                     IsActive = subscription.IsActive,
                                                     AutoRenewalIsEnabled = subscription.SubscriptionRenewal.AutoRenewalIsEnabled(),
                                                     UpgradingIsEnabled = subscription.SubscriptionRenewal.UpgradingIsEnabled(),
                                                     DowngradingIsEnabled = subscription.SubscriptionRenewal.DowngradingIsEnabled(),
                                                     IsSubscriptionResetUrlExists = !string.IsNullOrWhiteSpace(subscription.Product.SubscriptionResetUrl),
                                                     IsSubscriptionUpgradeUrlExists = !string.IsNullOrWhiteSpace(subscription.Product.SubscriptionUpgradeUrl),
                                                     IsSubscriptionDowngradeUrlExists = !string.IsNullOrWhiteSpace(subscription.Product.SubscriptionDowngradeUrl),
                                                     SubscriptionCycles = subscription.SubscriptionCycles.Select(SubscriptionCycle => new SubscriptionCycleDto
                                                     {
                                                         Id = SubscriptionCycle.Id,
                                                         StartDate = SubscriptionCycle.StartDate,
                                                         EndDate = SubscriptionCycle.EndDate,
                                                         CycleType = SubscriptionCycle.Type,

                                                     }),
                                                     Plan = new CustomLookupItemDto<Guid>
                                                     {
                                                         Id = subscription.Plan.Id,
                                                         SystemName = subscription.Plan.SystemName,
                                                         DisplayName = subscription.Plan.DisplayName,
                                                     },
                                                     PlanPrice = new PlanPriceDto
                                                     {
                                                         Id = subscription.PlanPrice.Id,
                                                         Cycle = subscription.PlanPrice.PlanCycle,
                                                         Price = subscription.PlanPrice.Price,
                                                     },
                                                     SubscriptionRenewal = subscription.SubscriptionRenewal == null ? null : new SubscriptionDetailsDto.SubscriptionRenewalDto
                                                     {
                                                         PlanDisplayName = subscription.SubscriptionRenewal.PlanDisplayName,
                                                         Type = subscription.SubscriptionRenewal.Type,
                                                         Cycle = subscription.SubscriptionRenewal.PlanCycle,
                                                         Price = subscription.SubscriptionRenewal.Price,
                                                         EditedDate = subscription.SubscriptionRenewal.ModificationDate,
                                                         CreatedDate = subscription.SubscriptionRenewal.CreationDate,
                                                         Comment = subscription.SubscriptionRenewal.Comment,
                                                         IsContinuousRenewal = subscription.SubscriptionRenewal.IsContinuousRenewal,
                                                         RenewalsCount = subscription.SubscriptionRenewal.RenewalsCount,
                                                         Status = subscription.SubscriptionRenewal.Status,
                                                         SubscriptionRenewalDate = subscription.SubscriptionRenewal.SubscriptionRenewalDate,
                                                     },
                                                     SubscriptionRenewalAction = subscription.ToSubscriptionRenewalAction(subscription.SubscriptionRenewal, subscription.Product),
                                                 })
                                                 .SingleOrDefaultAsync(cancellationToken);

            if (subscription is null)
            {
                return Result<SubscriptionDetailsDto>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            //subscription.HasSubscriptionFeaturesLimitsResettable = subscription.SubscriptionFeatures
            //                                                                    .Select(x => x.Feature.Reset)
            //                                                                    .Where(reset => FeatureResetManager.FromKey(reset).IsResettable())
            //                                                                    .Any();

            subscription.AutoRenewal = subscription.SubscriptionRenewal == null ? null :
                                            subscription.SubscriptionRenewal.Type != SubscriptionRenewalTypeEnum.AutoRenewal ? null :
                                            new SubscriptionDetailsDto.SubscriptionAutoRenewalDto
                                            {
                                                Cycle = subscription.SubscriptionRenewal.Cycle,
                                                Price = subscription.SubscriptionRenewal.Price,
                                                EditedDate = subscription.SubscriptionRenewal.EditedDate,
                                                CreatedDate = subscription.SubscriptionRenewal.CreatedDate,
                                                Comment = subscription.SubscriptionRenewal.Comment,
                                            };
            subscription.SubscriptionPlanChange = subscription.SubscriptionRenewal == null ? null :
                                    subscription.SubscriptionRenewal.Type == SubscriptionRenewalTypeEnum.AutoRenewal ? null :
                                     new SubscriptionDetailsDto.SubscriptionPlanChangingDto
                                     {
                                         PlanDisplayName = subscription.SubscriptionRenewal.PlanDisplayName,
                                         Type = subscription.SubscriptionRenewal.Type,
                                         Cycle = subscription.SubscriptionRenewal.Cycle,
                                         Price = subscription.SubscriptionRenewal.Price,
                                         EditedDate = subscription.SubscriptionRenewal.EditedDate,
                                         CreatedDate = subscription.SubscriptionRenewal.CreatedDate,
                                         Comment = subscription.SubscriptionRenewal.Comment,
                                     };







            subscription.IsPlanChangeAllowed = subscription.SubscriptionPlanChange is null &&
                                                        (subscription.SubscriptionPlanChangeStatus is null) &&
                                                         subscription.IsSubscriptionUpgradeUrlExists &&
                                                         subscription.IsSubscriptionDowngradeUrlExists;

            subscription.IsResettableAllowed = (subscription.LastResetDate is null || DateTime.UtcNow > subscription.LastResetDate.Value.AddHours(24)) &&
                                               (subscription.SubscriptionResetStatus is null ||
                                                subscription.SubscriptionResetStatus == SubscriptionResetStatus.Done) &&
                                                subscription.IsSubscriptionResetUrlExists;


            subscription.SubscriptionCycles = subscription.SubscriptionCycles.OrderByDescending(x => x.StartDate).ToList();

            return Result<SubscriptionDetailsDto>.Successful(subscription);
        }

        public async Task<Result<List<MySubscriptionListItemDto>>> GetSubscriptionsListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.Subscriptions.AsNoTracking()
                                                        .Where(x => _identityContextService.IsSuperAdmin() ||
                                                                    _dbContext.EntityAdminPrivileges
                                                                            .Any(a => a.UserId == userId &&
                                                                                      a.EntityId == x.TenantId &&
                                                                                      a.EntityType == EntityType.Tenant
                                                                                ))
                                                         .Select(subscription => new MySubscriptionListItemDto
                                                         {
                                                             Id = subscription.Id,
                                                             SubscriptionId = subscription.Id,
                                                             SubscriptionMode = subscription.SubscriptionMode,
                                                             SystemName = subscription.Tenant.SystemName,
                                                             DisplayName = subscription.Tenant.DisplayName,
                                                             IsActive = subscription.IsActive,
                                                             EndDate = subscription.EndDate,
                                                             StartDate = subscription.StartDate,
                                                             PlanPrice = new PlanPriceDto
                                                             {
                                                                 Id = subscription.PlanPriceId,
                                                                 Cycle = subscription.PlanPrice.PlanCycle,
                                                                 Price = subscription.PlanPrice.Price,
                                                             },
                                                             Plan = new Common.Models.CustomLookupItemDto<Guid>(subscription.PlanId, subscription.Plan.SystemName, subscription.Plan.DisplayName),
                                                             Product = new Common.Models.CustomLookupItemDto<Guid>(subscription.ProductId, subscription.Product.SystemName, subscription.Product.DisplayName),
                                                             CreatedDate = subscription.Tenant.CreationDate,
                                                             EditedDate = subscription.Tenant.ModificationDate,

                                                             AutoRenewalIsEnabled = subscription.SubscriptionRenewal.AutoRenewalIsEnabled(),
                                                             UpgradingIsEnabled = subscription.SubscriptionRenewal.UpgradingIsEnabled(),
                                                             DowngradingIsEnabled = subscription.SubscriptionRenewal.DowngradingIsEnabled(),
                                                             PlanChangingType = subscription.SubscriptionRenewal == null ||
                                                                                subscription.SubscriptionRenewal.Type == SubscriptionRenewalTypeEnum.AutoRenewal ? null :
                                                                                subscription.SubscriptionRenewal.Type,

                                                             PlanChangingIsEnabled = subscription.SubscriptionRenewal == null ||
                                                                                     subscription.SubscriptionRenewal.Type == SubscriptionRenewalTypeEnum.AutoRenewal ? false : true,
                                                             IsPlanChangeAllowed = subscription.ToSubscriptionRenewalAction(subscription.SubscriptionRenewal, subscription.Product).EnabelUpgrading &&
                                                                                    subscription.ToSubscriptionRenewalAction(subscription.SubscriptionRenewal, subscription.Product).EnabelDowngrading,
                                                             Trial = subscription.Trial == null ? null : new MySubscriptionListItemDto.TrialSubscriptionDto
                                                             {
                                                                 EndDate = subscription.Trial.EndDate,
                                                                 SelectedPlanId = subscription.Trial.SelectedPlanId,
                                                                 SelectedPlanPriceId = subscription.Trial.SelectedPlanPriceId,
                                                                 TrialPeriodInDays = subscription.Trial.TrialPeriodInDays,
                                                                 TrialPlanId = subscription.Trial.TrialPlanId,
                                                                 TrialPlanPriceId = subscription.Trial.TrialPlanPriceId,
                                                             },
                                                             SubscriptionRenewalAction = subscription.ToSubscriptionRenewalAction(subscription.SubscriptionRenewal, subscription.Product),


                                                         })
                                                         .OrderByDescending(x => x.CreatedDate)
                                                         .ToListAsync(cancellationToken);

            return Result<List<MySubscriptionListItemDto>>.Successful(tenants);
        }

        public async Task<Result<List<SubscriptionListItemDto>>> GetSubscriptionsListByProductIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.Subscriptions.AsNoTracking()
                                                 .Where(x => x.ProductId == productId)
                                                 .Select(x => new SubscriptionListItemDto
                                                 {
                                                     Id = x.Id,
                                                     SubscriptionId = x.Id,
                                                     TenantId = x.TenantId,
                                                     SystemName = x.Tenant.SystemName,
                                                     HealthCheckUrl = x.HealthCheckUrl,
                                                     HealthCheckUrlIsOverridden = x.HealthCheckUrlIsOverridden,
                                                     DisplayName = x.Tenant.DisplayName,
                                                     Status = x.Status,
                                                     IsActive = x.IsActive,
                                                     EndDate = x.EndDate,
                                                     StartDate = x.StartDate,
                                                     Plan = new Common.Models.CustomLookupItemDto<Guid>(x.PlanId, x.Plan.SystemName, x.Plan.DisplayName),
                                                     CreatedDate = x.Tenant.CreationDate,
                                                     EditedDate = x.Tenant.ModificationDate,
                                                 })
                                                 .ToListAsync(cancellationToken);

            return Result<List<SubscriptionListItemDto>>.Successful(tenants);
        }

        #endregion



    }

    public static class SubscriptionHelper
    {
        public static SubscriptionRenewalAction ToSubscriptionRenewalAction(this Subscription subscription, SubscriptionRenewal subscriptionRenewal, Product product)
        {
            return new SubscriptionRenewalAction
            {
                EnableAutoRenual = subscriptionRenewal == null || subscriptionRenewal.Type != SubscriptionRenewalTypeEnum.AutoRenewal,
                EnabelUpgrading = subscription.SubscriptionMode == SubscriptionMode.Standard &&
                                  subscriptionRenewal == null &&
                                  !string.IsNullOrWhiteSpace(product.SubscriptionUpgradeUrl),
                EnabelDowngrading = subscription.SubscriptionMode == SubscriptionMode.Standard &&
                                    subscriptionRenewal == null &&
                                    !string.IsNullOrWhiteSpace(product.SubscriptionDowngradeUrl),
                CancelUpgrading = subscriptionRenewal.UpgradingIsEnabled(),
                CancelDowngrading = subscriptionRenewal.DowngradingIsEnabled(),
                CancelAutoRenual = subscriptionRenewal.AutoRenewalIsEnabled(),
            };
        }
        public static bool AutoRenewalIsEnabled(this SubscriptionRenewal subscriptionRenewal)
        {
            return subscriptionRenewal != null &&
                    (subscriptionRenewal.Type == SubscriptionRenewalTypeEnum.AutoRenewal ||
                     subscriptionRenewal.IsContinuousRenewal ||
                     subscriptionRenewal.RenewalsCount > 0);
        }
        public static bool UpgradingIsEnabled(this SubscriptionRenewal subscriptionRenewal)
        {
            return subscriptionRenewal != null && subscriptionRenewal.Type == SubscriptionRenewalTypeEnum.Upgrade;
        }
        public static bool DowngradingIsEnabled(this SubscriptionRenewal subscriptionRenewal)
        {
            return subscriptionRenewal != null && subscriptionRenewal.Type == SubscriptionRenewalTypeEnum.Downgrade;
        }
    }
}
