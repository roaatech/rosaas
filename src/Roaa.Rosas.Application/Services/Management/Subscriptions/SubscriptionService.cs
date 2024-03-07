using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Utilities;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public class SubscriptionService : ISubscriptionService
    {
        #region Props 
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;
        private readonly IMediator _mediator;
        private DateTime _date;
        #endregion


        #region Corts
        public SubscriptionService(ILogger<SubscriptionService> logger,
                                   IIdentityContextService identityContextService,
                                   IRosasDbContext dbContext,
                                   IPublisher publisher,
                                   IMediator mediator)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _dbContext = dbContext;
            _publisher = publisher;
            _mediator = mediator;
            _date = DateTime.UtcNow;
        }

        #endregion


        #region Services 


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
                                                             SystemName = subscription.Tenant.SystemName,
                                                             DisplayName = subscription.Tenant.DisplayName,
                                                             IsActive = subscription.IsActive,
                                                             EndDate = subscription.EndDate,
                                                             StartDate = subscription.StartDate,
                                                             Plan = new Common.Models.CustomLookupItemDto<Guid>(subscription.PlanId, subscription.Plan.SystemName, subscription.Plan.DisplayName),
                                                             Product = new Common.Models.CustomLookupItemDto<Guid>(subscription.ProductId, subscription.Product.SystemName, subscription.Product.DisplayName),
                                                             CreatedDate = subscription.Tenant.CreationDate,
                                                             EditedDate = subscription.Tenant.ModificationDate,
                                                             AutoRenewalIsEnabled = subscription.AutoRenewal == null ? false : true,
                                                             PlanChangingIsEnabled = subscription.SubscriptionPlanChanging == null ? false : true,
                                                             PlanChangingType = subscription.SubscriptionPlanChanging == null ? null : subscription.SubscriptionPlanChanging.Type,
                                                         })
                                                         .ToListAsync(cancellationToken);

            return Result<List<MySubscriptionListItemDto>>.Successful(tenants);
        }

        public async Task<Result<List<SubscriptionListItemDto>>> GetSubscriptionsListByProductIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.Subscriptions.AsNoTracking()
                                                 .Where(x => x.ProductId == productId)
                                                 .Select(GetSubscriptionDtoSelector())
                                                 .ToListAsync(cancellationToken);

            return Result<List<SubscriptionListItemDto>>.Successful(tenants);
        }


        public Expression<Func<Subscription, SubscriptionListItemDto>> GetSubscriptionDtoSelector()
        {
            return x => new SubscriptionListItemDto
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

            };
        }
        public async Task<Result> DeactivateSubscriptionDueToNonPaymentAsync(int periodTimeAfterEndDateInHours, CancellationToken cancellationToken = default)
        {
            var currentDate = DateTime.UtcNow;
            var toDate = DateTime.UtcNow.AddHours(periodTimeAfterEndDateInHours);
            var subscriptions = await _dbContext.Subscriptions
                                                .Where(x => x.StartDate <= currentDate &&
                                                            x.EndDate < toDate &&
                                                            !x.IsActive)
                                                .ToListAsync();


            foreach (var subscription in subscriptions)
            {




                await _mediator.Send(new ChangeTenantStatusByIdCommand(subscription.TenantId,
                                                                       TenantStatus.SendingDeactivationRequest,
                                                                       subscription.ProductId,
                                                                       "Deactivating the tenant due to non-payment of the its subscription."),
                                         cancellationToken);

            }
            return Result.Successful();
        }

        public async Task<Result> ResetSubscriptionsFeaturesAsync(CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var subscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                        .Include(x => x.Feature)
                                                        .Where(x => x.Subscription.StartDate <= date &&
                                                                    x.Subscription.EndDate > date &&
                                                                    x.Subscription.IsActive &&
                                                                    x.RemainingUsage != null &&
                                                                    x.StartDate <= date &&
                                                                    x.EndDate != null &&
                                                                    x.EndDate < date)
                                                       .ToListAsync();
            return await ResetSubscriptionsFeaturesAsync(subscriptionFeatures: subscriptionFeatures,
                                                         comment: null,
                                                         systemComment: "Reset by the System",
                                                         cancellationToken: cancellationToken);
        }

        public async Task<Result> ResetSubscriptionsFeaturesAsync(List<SubscriptionFeature> subscriptionFeatures, string? comment, string? systemComment, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;

            if (subscriptionFeatures.Any())
            {


                var featureCyclesIds = subscriptionFeatures
                                                .Select(x => x.SubscriptionFeatureCycleId)
                                                .ToList();

                var featureCycles = await _dbContext.SubscriptionFeatureCycles
                                                           .Where(x => featureCyclesIds.Contains(x.Id))
                                                           .ToListAsync();



                if (featureCycles.Select(x => x.FeatureReset).Where(reset => FeatureResetManager.FromKey(reset).IsResettable()).Any())
                {

                    var subscriptionsIds = new List<Guid>();

                    // Adds a new cycle to the subscription's features
                    foreach (var subscriptionFeature in subscriptionFeatures)
                    {
                        var cycle = featureCycles
                                        .Where(x => x.Id == subscriptionFeature.SubscriptionFeatureCycleId)
                                        .FirstOrDefault();

                        if (FeatureResetManager.FromKey(cycle.FeatureReset).IsResettable())
                        {
                            var featureResetManager = FeatureResetManager.FromKey(cycle.FeatureReset);

                            // Adds a new cycle to the current subscription's feature 
                            var subscriptionFeatureCycle = new SubscriptionFeatureCycle()
                            {
                                Id = Guid.NewGuid(),
                                StartDate = featureResetManager.GetStartDate(_date),
                                EndDate = featureResetManager.GetExpiryDate(date),
                                SubscriptionId = subscriptionFeature.SubscriptionId,
                                SubscriptionCycleId = cycle.SubscriptionCycleId,
                                SubscriptionFeatureId = subscriptionFeature.Id,
                                FeatureId = cycle.FeatureId,
                                PlanFeatureId = cycle.PlanFeatureId,
                                Limit = cycle.Limit,
                                FeatureReset = cycle.FeatureReset,
                                FeatureType = cycle.FeatureType,
                                FeatureUnit = cycle.FeatureUnit,
                                TotalUsage = cycle.Limit is null ? null : 0,
                                RemainingUsage = cycle.Limit,
                                PlanCycle = cycle.PlanCycle,
                                FeatureDisplayName = cycle.FeatureDisplayName,
                                CreatedByUserId = _identityContextService.GetActorId(),
                                ModifiedByUserId = _identityContextService.GetActorId(),
                                CreationDate = date,
                                ModificationDate = date,
                            };


                            // update the subscription's feature by the new cycle of current subscription's feature 
                            subscriptionFeature.SubscriptionFeatureCycleId = subscriptionFeatureCycle.Id;
                            subscriptionFeature.StartDate = subscriptionFeatureCycle.StartDate;
                            subscriptionFeature.EndDate = subscriptionFeatureCycle.EndDate;
                            subscriptionFeature.RemainingUsage = subscriptionFeatureCycle.Limit;

                            _dbContext.SubscriptionFeatureCycles.Add(subscriptionFeatureCycle);


                            subscriptionsIds.Add(subscriptionFeature.SubscriptionId);
                        }

                    }
                    subscriptionsIds = subscriptionsIds.Distinct().ToList();


                    // retrieving the subscriptions 
                    var subscriptions = await _dbContext.Subscriptions
                                                        .Where(x => subscriptionsIds.Contains(x.Id))
                                                        .ToListAsync();

                    // update the LastLimitsResetDate property value of subscription's 
                    foreach (var subscription in subscriptions)
                    {


                        subscription.LastLimitsResetDate = date;

                        var subscriptionFeatureItems = subscriptionFeatures
                                                            .Where(x => x.SubscriptionId == subscription.Id)
                                                            .Select(x => new SubscriptionFeatureItemModel(
                                                                                            subscriptionFeatureId: x.Id,
                                                                                            name: x.Feature?.SystemName ?? ""))
                                                            .ToList();

                        subscription.AddDomainEvent(new SubscriptionFeaturesLimitsResetEvent(
                                                                     subscriptionFeatures: subscriptionFeatureItems,
                                                                     subscription: subscription,
                                                                     comment: comment,
                                                                     systemComment: systemComment));

                    }



                    await _dbContext.SaveChangesAsync();
                }
            }

            return Result.Successful();
        }

        public async Task<List<Subscription>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Subscriptions
                                                .Where(x => x.StartDate <= _date &&
                                                            x.EndDate < _date &&
                                                            x.IsActive
                                                          )
                                                .ToListAsync(cancellationToken);
        }

        public async Task<Result> TryToExtendOrSuspendSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            _date = DateTime.UtcNow;

            var expiredSubscriptions = await GetExpiredSubscriptionsAsync(cancellationToken);

            if (!expiredSubscriptions.Any())
            {
                return Result.Successful();
            }

            var expiredSbscriptionsIds = expiredSubscriptions.Select(x => x.Id).ToList();

            var subscriptionsFeatures = await _dbContext.SubscriptionFeatures
                                                          .Where(x => expiredSbscriptionsIds.Contains(x.SubscriptionId))
                                                          .ToListAsync();

            // Retrieving subscriptions auto-renewals from database
            var subscriptionAutoRenewals = await _dbContext.SubscriptionAutoRenewals
                                                         .Where(x => expiredSbscriptionsIds.Contains(x.SubscriptionId))
                                                         .ToListAsync();

            // Retrieving subscriptions plans changes from database
            var subscriptionPlanChanges = await _dbContext.SubscriptionPlanChanges
                                                     .Where(x => expiredSbscriptionsIds.Contains(x.SubscriptionId))
                                                     .ToListAsync();


            foreach (var subscription in expiredSubscriptions)
            {
                var subscriptionFeatures = subscriptionsFeatures
                                                .Where(x => x.SubscriptionId == subscription.Id)
                                                .ToList();

                #region First Case (Subscription Plan Changing)
                var subscriptionPlanChanging = subscriptionPlanChanges.Where(x => x.SubscriptionId == subscription.Id).SingleOrDefault();

                // Changing subscription plan that needs to change its plan 
                if (subscriptionPlanChanging is not null)
                {
                    await PrepareSubscriptionToChangePlanAsync(subscription, subscriptionPlanChanging, cancellationToken);
                    continue;
                }
                #endregion

                #region Second Case (Subscription Auto Renewal)
                var autoRenewal = subscriptionAutoRenewals.Where(x => x.SubscriptionId == subscription.Id).SingleOrDefault();

                // Renewing subscription plan that has enabled auto-renewal
                if (autoRenewal is not null)
                {
                    await RenewSubscriptionAsync(subscription, autoRenewal, subscriptionFeatures, cancellationToken);
                    continue;
                }
                #endregion

                if (subscription.SubscriptionMode == SubscriptionMode.Trial)
                {
                    subscription.SubscriptionMode = SubscriptionMode.PendingToNormal;
                }

                #region 3th Case (Alternative Plan)

                var plan = await _dbContext.Plans
                                 .AsNoTracking()
                                 .Where(x => x.Id == subscription.PlanId)
                                 .Select(x => new
                                 {
                                     x.Id,
                                     x.AlternativePlanId,
                                     x.AlternativePlanPriceId,
                                     x.TrialPeriodInDays
                                 })
                                 .SingleOrDefaultAsync(cancellationToken);

                if (plan.AlternativePlanId is not null)
                {
                    var alternativePlanPrice = await _dbContext.PlanPrices
                                                               .AsNoTracking()
                                                               .Where(x => x.Id == plan.AlternativePlanPriceId &&
                                                                           x.PlanId == plan.AlternativePlanId)
                                                               .Include(x => x.Plan)
                                                               .SingleOrDefaultAsync(cancellationToken);

                    var newSubscriptionPlanChanging = new SubscriptionPlanChanging
                    {
                        Id = subscription.Id,
                        Type = PlanChangingType.Downgrade,
                        SubscriptionId = subscription.Id,
                        PlanPriceId = alternativePlanPrice.Id,
                        PlanId = alternativePlanPrice.PlanId,
                        PlanCycle = alternativePlanPrice.PlanCycle,
                        Price = alternativePlanPrice.Price,
                        PlanDisplayName = alternativePlanPrice.Plan.DisplayName ?? "",
                        IsPaid = true,
                        //Comment = Comment, 
                        CreationDate = _date,
                        ModificationDate = _date,
                    };
                    _dbContext.SubscriptionPlanChanges.Add(newSubscriptionPlanChanging);

                    await PrepareSubscriptionToChangePlanAsync(subscription, newSubscriptionPlanChanging, cancellationToken);
                    continue;
                }
                #endregion

                #region 4th Case

                await SuspendSubscriptionAsync(subscription, cancellationToken);

                #endregion

            }



            return Result.Successful();
        }


        /// <summary>
        /// Changing subscription plan that needs to change its plan 
        /// </summary> 
        public async Task<Result> ChangeSubscriptionPlanAsync(Subscription subscription,
                                                               CancellationToken cancellationToken = default)
        {
            var subscriptionPlanChanging = await _dbContext.SubscriptionPlanChanges
                                              .Where(x => x.SubscriptionId == subscription.Id)
                                              .SingleOrDefaultAsync(cancellationToken);

            var previousSubscriptionCycleId = subscription.SubscriptionCycleId;

            var previousSubscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                          .Where(x => x.SubscriptionId == subscription.Id)
                                                          .ToListAsync();

            _dbContext.SubscriptionFeatures.RemoveRange(previousSubscriptionFeatures);

            var planFeaturesInfo = await _dbContext.PlanFeatures
                                         .AsNoTracking()
                                         .Where(x => x.PlanId == subscriptionPlanChanging.PlanId)
                                         .Select(x => new PlanFeatureInfoModel
                                         {
                                             PlanFeatureId = x.Id,
                                             FeatureId = x.FeatureId,
                                             FeatureUnit = x.FeatureUnit,
                                             PlanId = x.PlanId,
                                             Limit = x.Limit,
                                             FeatureDisplayName = x.Feature.DisplayName,
                                             FeatureType = x.Feature.Type,
                                             FeatureReset = x.FeatureReset,
                                         })
                                         .ToListAsync(cancellationToken);

            var subscriptionFeatures = planFeaturesInfo.Select(x =>
                                            BuildSubscriptionFeatureEntity(subscription.Id,
                                                                            x.FeatureId,
                                                                            x.PlanFeatureId,
                                                                            x.FeatureReset,
                                                                            x.Limit))
                                                       .ToList();

            _dbContext.SubscriptionFeatures.AddRange(subscriptionFeatures);

            PrepareToExtendSubscription(subscription,
                            subscriptionFeatures,
                            planFeaturesInfo,
                            subscriptionPlanChanging.PlanId,
                            subscriptionPlanChanging.PlanPriceId,
                            subscriptionPlanChanging.PlanCycle,
                            subscriptionPlanChanging.Price,
                            subscriptionPlanChanging.PlanDisplayName);



            subscription.PlanId = subscriptionPlanChanging.PlanId;
            subscription.PlanPriceId = subscriptionPlanChanging.PlanPriceId;
            subscription.SubscriptionPlanChangeStatus = SubscriptionPlanChangeStatus.Done;
            subscription.ModificationDate = DateTime.UtcNow;
            subscription.AddDomainEvent(new SubscriptionDowngradeAppliedDoneEvent(subscription));


            var subscriptionPlanChangeHistory = new SubscriptionPlanChangeHistory
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscriptionPlanChanging.SubscriptionId,
                PlanId = subscriptionPlanChanging.PlanId,
                PlanPriceId = subscriptionPlanChanging.PlanPriceId,
                Type = subscriptionPlanChanging.Type,
                PlanCycle = subscriptionPlanChanging.PlanCycle,
                Price = subscriptionPlanChanging.Price,
                PlanChangeEnabledByUserId = subscriptionPlanChanging.ModifiedByUserId,
                PlanChangeEnabledDate = subscriptionPlanChanging.ModificationDate,
                Comment = subscriptionPlanChanging.Comment,
                ChangeDate = DateTime.UtcNow,
            };

            _dbContext.SubscriptionPlanChangeHistories.Remove(subscriptionPlanChangeHistory);

            _dbContext.SubscriptionPlanChanges.Remove(subscriptionPlanChanging);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> ResetSubscriptionPlanAsync(Subscription subscription,
                                                                Guid planId,
                                                                Guid planPriceId,
                                                                bool? isActive = null,
                                                                SubscriptionMode? subscriptionMode = null,
                                                                CancellationToken cancellationToken = default)
        {
            var previousSubscriptionCycleId = subscription.SubscriptionCycleId;

            var previousSubscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                          .Where(x => x.SubscriptionId == subscription.Id)
                                                          .ToListAsync();

            _dbContext.SubscriptionFeatures.RemoveRange(previousSubscriptionFeatures);

            var planFeaturesInfo = await _dbContext.PlanFeatures
                                         .AsNoTracking()
                                         .Where(x => x.PlanId == planId)
                                         .Select(x => new PlanFeatureInfoModel
                                         {
                                             PlanFeatureId = x.Id,
                                             FeatureId = x.FeatureId,
                                             FeatureUnit = x.FeatureUnit,
                                             PlanId = x.PlanId,
                                             Limit = x.Limit,
                                             FeatureDisplayName = x.Feature.DisplayName,
                                             FeatureType = x.Feature.Type,
                                             FeatureReset = x.FeatureReset,
                                         })
                                         .ToListAsync(cancellationToken);

            var subscriptionFeatures = planFeaturesInfo.Select(x =>
                                            BuildSubscriptionFeatureEntity(subscription.Id,
                                                                            x.FeatureId,
                                                                            x.PlanFeatureId,
                                                                            x.FeatureReset,
                                                                            x.Limit))
                                                       .ToList();


            var planPrice = await _dbContext.PlanPrices
                                         .AsNoTracking()
                                         .Include(x => x.Plan)
                                         .Where(x => x.Id == planPriceId)
                                         .SingleOrDefaultAsync(cancellationToken);

            _dbContext.SubscriptionFeatures.AddRange(subscriptionFeatures);

            PrepareToExtendSubscription(subscription,
                            subscriptionFeatures,
                            planFeaturesInfo,
                            planId,
                            planPriceId,
                            planPrice.PlanCycle,
                            planPrice.Price,
                            planPrice.Plan.DisplayName);



            subscription.PlanId = planId;
            subscription.PlanPriceId = planPriceId;
            subscription.ModificationDate = DateTime.UtcNow;

            if (isActive is not null)
            {
                subscription.IsActive = isActive.Value;
            }
            if (subscriptionMode is not null)
            {
                subscription.SubscriptionMode = subscriptionMode.Value;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        public async Task<Result> PrepareSubscriptionToChangePlanAsync(Subscription subscription,
                                                                       SubscriptionPlanChanging subscriptionPlanChanging,
                                                                       CancellationToken cancellationToken = default)
        {
            if (subscription.SubscriptionPlanChangeStatus != null &&
                subscription.SubscriptionPlanChangeStatus == SubscriptionPlanChangeStatus.Pending)
            {
                return Result.Successful();
            }

            subscription.AddDomainEvent(new SubscriptionPlanChangePreparedEvent(
                                          subscription,
                                          subscriptionPlanChanging,
                                          Guid.Empty));
            subscription.SubscriptionPlanChangeStatus = SubscriptionPlanChangeStatus.Pending;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        /// <summary>
        /// Renewing subscription plan that has enabled auto-renewal
        /// </summary> 
        public async Task<Result> RenewSubscriptionAsync(Subscription subscription,
                                                         SubscriptionAutoRenewal autoRenewal,
                                                         List<SubscriptionFeature> subscriptionFeatures,
                                                         CancellationToken cancellationToken = default)
        {

            var subscriptionFeatureCyclesIds = subscriptionFeatures.Select(x => x.SubscriptionFeatureCycleId).ToList();

            var planFeaturesInfo = await _dbContext.SubscriptionFeatureCycles
                                                               .Where(x => subscriptionFeatureCyclesIds.Contains(x.Id))
                                                               .Select(x => new PlanFeatureInfoModel
                                                               {
                                                                   PlanFeatureId = x.Id,
                                                                   FeatureId = x.FeatureId,
                                                                   FeatureUnit = x.FeatureUnit,
                                                                   PlanId = autoRenewal.PlanId,
                                                                   Limit = x.Limit,
                                                                   FeatureDisplayName = x.FeatureDisplayName,
                                                                   FeatureType = x.FeatureType,
                                                                   FeatureReset = x.FeatureReset,
                                                               })
                                                               .ToListAsync(cancellationToken);
            PrepareToExtendSubscription(subscription,
                                        subscriptionFeatures,
                                        planFeaturesInfo,
                                        autoRenewal.PlanId,
                                        autoRenewal.PlanPriceId,
                                        autoRenewal.PlanCycle,
                                        autoRenewal.Price,
                                        autoRenewal.PlanDisplayName);

            subscription.AddDomainEvent(new SubscriptionRenewedEvent(
                                                     subscription,
                                                     autoRenewal,
                                                     "The Subscription Is Automatically Renewed."));


            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        public async Task<Result> SuspendSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
        {
            string systemComment = "Setting the Subscription As Inactive for the tenant due to non-renewal.";

            SetSubscriptionAsInactive(subscription, systemComment, _date);

            subscription.AddDomainEvent(new SubscriptionWasSetAsInactiveDueToUnpaideEvent(subscription, systemComment));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        private void PrepareToExtendSubscription(Subscription subscription,
                                      List<SubscriptionFeature> subscriptionFeatures,
                                      List<PlanFeatureInfoModel> planFeaturesInfo,
                                      Guid planId,
                                      Guid planPriceId,
                                      PlanCycle planCycle,
                                      decimal planPrice,
                                      string planDisplayName)
        {
            // #1
            var subscriptionCycle = BuildSubscriptionCycleEntity(subscription: subscription,
                                                                 planId: planId,
                                                                 planPriceId: planPriceId,
                                                                 planCycle: planCycle,
                                                                 planPrice: planPrice,
                                                                 planDisplayName: planDisplayName);
            // #2
            UpdateSubscriptionEntity(subscription: subscription,
                                     subscriptionCycleId: subscriptionCycle.Id,
                                     startDate: subscriptionCycle.StartDate,
                                     endDate: subscriptionCycle.EndDate);

            foreach (var subscriptionFeature in subscriptionFeatures)
            {

                var planFeatureInfo = planFeaturesInfo.Where(x => x.PlanFeatureId == subscriptionFeature.PlanFeatureId).SingleOrDefault();

                // #3
                var subscriptionFeatureCycle = BuildSubscriptionFeatureCycleEntity(subscriptionFeature: subscriptionFeature,
                                                                                   planFeatureInfo: planFeatureInfo,
                                                                                   planCycle: planCycle,
                                                                                   subscriptionCycleId: subscriptionCycle.Id);
                // #4
                UpdateSubscriptionFeatureEntity(subscriptionFeature: subscriptionFeature,
                                         subscriptionFeatureCycleId: subscriptionFeatureCycle.Id,
                                         startDate: subscriptionFeatureCycle.StartDate,
                                         endDate: subscriptionFeatureCycle.EndDate,
                                         limit: subscriptionFeatureCycle.Limit);

                _dbContext.SubscriptionFeatureCycles.Add(subscriptionFeatureCycle);
            }



            _dbContext.SubscriptionCycles.Add(subscriptionCycle);
        }

        private SubscriptionFeature BuildSubscriptionFeatureEntity(Guid subscriptionId, Guid featureId, Guid planFeatureId, FeatureReset featureReset, int? limit)
        {
            var subscriptionFeature = new SubscriptionFeature
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscriptionId,
                //SubscriptionFeatureCycleId = f.GeneratedSubscriptionFeatureCycleId,
                StartDate = _date,
                EndDate = FeatureResetManager.FromKey(featureReset).GetExpiryDate(_date),
                FeatureId = featureId,
                PlanFeatureId = planFeatureId,
                RemainingUsage = limit,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreationDate = _date,
                ModificationDate = _date,
            };

            return subscriptionFeature;
        }

        private SubscriptionCycle BuildSubscriptionCycleEntity(Subscription subscription, Guid planId, Guid planPriceId, PlanCycle planCycle, decimal planPrice, string planDisplayName)
        {
            var subscriptionCycle = new SubscriptionCycle()
            {
                Id = Guid.NewGuid(),
                StartDate = _date,
                EndDate = PlanCycleManager.FromKey(planCycle).CalculateExpiryDate(_date, null),
                SubscriptionId = subscription.Id,
                TenantId = subscription.TenantId,
                PlanId = planId,
                PlanPriceId = planPriceId,
                ProductId = subscription.ProductId,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreationDate = _date,
                ModificationDate = _date,
                PlanDisplayName = planDisplayName,
                Cycle = planCycle,
                Price = planPrice,
                Type = SubscriptionCycleType.Normal,
            };

            return subscriptionCycle;
        }

        private void UpdateSubscriptionEntity(Subscription subscription, Guid subscriptionCycleId, DateTime startDate, DateTime? endDate)
        {
            subscription.SubscriptionCycleId = subscriptionCycleId;
            subscription.StartDate = startDate;
            subscription.EndDate = endDate;
        }

        private SubscriptionFeatureCycle BuildSubscriptionFeatureCycleEntity(
                                                            SubscriptionFeature subscriptionFeature,
                                                            PlanFeatureInfoModel planFeatureInfo,
                                                            PlanCycle planCycle,
                                                            Guid subscriptionCycleId)
        {
            var subscriptionFeatureCycle = new SubscriptionFeatureCycle()
            {
                Id = Guid.NewGuid(),
                StartDate = subscriptionFeature.StartDate,
                EndDate = subscriptionFeature.EndDate,
                SubscriptionId = subscriptionFeature.SubscriptionId,
                SubscriptionCycleId = subscriptionCycleId,
                SubscriptionFeatureId = subscriptionFeature.Id,
                FeatureId = subscriptionFeature.FeatureId,
                PlanFeatureId = subscriptionFeature.PlanFeatureId,
                Limit = planFeatureInfo.Limit,
                FeatureReset = planFeatureInfo.FeatureReset,
                FeatureType = planFeatureInfo.FeatureType,
                FeatureUnit = planFeatureInfo.FeatureUnit,
                TotalUsage = planFeatureInfo.Limit is null ? null : 0,
                RemainingUsage = planFeatureInfo.Limit,
                PlanCycle = planCycle,
                FeatureDisplayName = planFeatureInfo.FeatureDisplayName,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreationDate = _date,
                ModificationDate = _date,
            };

            return subscriptionFeatureCycle;
        }

        private void UpdateSubscriptionFeatureEntity(SubscriptionFeature subscriptionFeature, Guid subscriptionFeatureCycleId, DateTime? startDate, DateTime? endDate, int? limit)
        {
            subscriptionFeature.SubscriptionFeatureCycleId = subscriptionFeatureCycleId;
            subscriptionFeature.StartDate = startDate;
            subscriptionFeature.EndDate = endDate;
            subscriptionFeature.RemainingUsage = limit;
        }

        private void SetSubscriptionAsInactive(Subscription subscription, string systemComment, DateTime date)
        {
            subscription.IsActive = false;
            subscription.ModificationDate = date;
            subscription.Comment = systemComment;
        }

        public async Task<Result> Temp__RenewSubscriptionsAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var subscriptions = await _dbContext.Subscriptions
                                                .Include(x => x.Plan)
                                                .Include(x => x.PlanPrice)
                                                .Include(x => x.SubscriptionFeatures)
                                                .Where(x => x.Id == subscriptionId)
                                                .ToListAsync();

            if (subscriptions.Any())
            {
                var cyclesIds = subscriptions.Select(x => x.SubscriptionCycleId).ToList();

                var cycles = await _dbContext.SubscriptionCycles
                                                           .Where(x => cyclesIds.Contains(x.Id))
                                                           .ToListAsync();

                foreach (var subscription in subscriptions)
                {
                    var cycle = cycles.Where(x => x.Id == subscription.SubscriptionCycleId).FirstOrDefault();

                    var subscriptionCycle = new SubscriptionCycle()
                    {
                        Id = Guid.NewGuid(),
                        StartDate = date,
                        EndDate = PlanCycleManager.FromKey(subscription.PlanPrice.PlanCycle).CalculateExpiryDate(date, null),
                        TenantId = subscription.TenantId,
                        PlanId = subscription.PlanId,
                        PlanPriceId = subscription.PlanPriceId,
                        ProductId = subscription.ProductId,
                        Cycle = subscription.PlanPrice.PlanCycle,
                        PlanDisplayName = subscription.Plan.DisplayName,
                        CreatedByUserId = subscription.CreatedByUserId,
                        ModifiedByUserId = subscription.ModifiedByUserId,
                        CreationDate = date,
                        ModificationDate = date,
                        Price = subscription.PlanPrice.Price,
                        SubscriptionId = subscription.Id,
                        Type = SubscriptionCycleType.Normal,
                    };


                    subscription.SubscriptionCycleId = subscriptionCycle.Id;
                    subscription.StartDate = subscriptionCycle.StartDate;
                    subscription.EndDate = subscriptionCycle.EndDate;


                    if (subscription.SubscriptionFeatures.Any())
                    {
                        var featureCyclesIds = subscription.SubscriptionFeatures.Select(x => x.SubscriptionFeatureCycleId).ToList();

                        var featureCycles = await _dbContext.SubscriptionFeatureCycles
                                                                   .Where(x => featureCyclesIds.Contains(x.Id))
                                                                   .ToListAsync();

                        foreach (var subscriptionFeature in subscription.SubscriptionFeatures)
                        {
                            var featureCycle = featureCycles.Where(x => x.Id == subscriptionFeature.SubscriptionFeatureCycleId).FirstOrDefault();

                            var subscriptionFeatureCycle = new SubscriptionFeatureCycle()
                            {
                                Id = Guid.NewGuid(),
                                StartDate = date,
                                EndDate = FeatureResetManager.FromKey(featureCycle.FeatureReset).GetExpiryDate(date),
                                SubscriptionId = featureCycle.SubscriptionId,
                                SubscriptionCycleId = subscriptionCycle.Id,
                                SubscriptionFeatureId = subscriptionFeature.Id,
                                FeatureId = featureCycle.FeatureId,
                                PlanFeatureId = featureCycle.PlanFeatureId,
                                Limit = featureCycle.Limit,
                                FeatureReset = featureCycle.FeatureReset,
                                FeatureType = featureCycle.FeatureType,
                                FeatureUnit = featureCycle.FeatureUnit,
                                TotalUsage = featureCycle.Limit is null ? null : 0,
                                RemainingUsage = featureCycle.Limit,
                                PlanCycle = featureCycle.PlanCycle,
                                FeatureDisplayName = featureCycle.FeatureDisplayName,
                                CreatedByUserId = featureCycle.CreatedByUserId,
                                ModifiedByUserId = featureCycle.ModifiedByUserId,
                                CreationDate = date,
                                ModificationDate = date,
                            };

                            subscriptionFeature.SubscriptionFeatureCycleId = subscriptionFeatureCycle.Id;
                            subscriptionFeature.StartDate = subscriptionFeatureCycle.StartDate;
                            subscriptionFeature.EndDate = subscriptionFeatureCycle.EndDate;
                            subscriptionFeature.RemainingUsage = subscriptionFeatureCycle.Limit;

                            _dbContext.SubscriptionFeatureCycles.Add(subscriptionFeatureCycle);
                        }

                    }



                    _dbContext.SubscriptionCycles.Add(subscriptionCycle);
                }

                await _dbContext.SaveChangesAsync();
            }

            return Result.Successful();
        }

        public async Task<Result> Temp__EndSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var subscription = await _dbContext.Subscriptions
                                                .Where(x => x.Id == subscriptionId)
                                                .SingleOrDefaultAsync();

            subscription.StartDate = DateTime.UtcNow.AddDays(-2);
            subscription.EndDate = DateTime.UtcNow.AddDays(-1);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Successful();
        }
        #endregion

        /// <summary>
        /// Changing subscription plan that needs to change its plan 
        /// </summary> 
        public async Task<Result> BACKUP_ChangeSubscriptionPlanAsync(Subscription subscription,
                                                               SubscriptionPlanChanging subscriptionPlanChanging,
                                                               List<SubscriptionFeature> subscriptionFeatures,
                                                               CancellationToken cancellationToken = default)
        {
            var previousSubscriptionCycleId = subscription.SubscriptionCycleId;

            _dbContext.SubscriptionFeatures.RemoveRange(subscriptionFeatures);

            var planFeaturesInfo = await _dbContext.PlanFeatures
                                         .AsNoTracking()
                                         .Where(x => x.PlanId == subscriptionPlanChanging.PlanId)
                                         .Select(x => new PlanFeatureInfoModel
                                         {
                                             PlanFeatureId = x.Id,
                                             FeatureId = x.FeatureId,
                                             FeatureUnit = x.FeatureUnit,
                                             PlanId = x.PlanId,
                                             Limit = x.Limit,
                                             FeatureDisplayName = x.Feature.DisplayName,
                                             FeatureType = x.Feature.Type,
                                             FeatureReset = x.FeatureReset,
                                         })
                                         .ToListAsync(cancellationToken);

            subscriptionFeatures = planFeaturesInfo.Select(x =>
                                    BuildSubscriptionFeatureEntity(subscription.Id,
                                                                    x.FeatureId,
                                                                    x.PlanFeatureId,
                                                                    x.FeatureReset,
                                                                    x.Limit))
                                                    .ToList();

            _dbContext.SubscriptionFeatures.AddRange(subscriptionFeatures);

            PrepareToExtendSubscription(subscription,
                            subscriptionFeatures,
                            planFeaturesInfo,
                            subscriptionPlanChanging.PlanId,
                            subscriptionPlanChanging.PlanPriceId,
                            subscriptionPlanChanging.PlanCycle,
                            subscriptionPlanChanging.Price,
                            subscriptionPlanChanging.PlanDisplayName);

            subscription.AddDomainEvent(new SubscriptionPlanChangePreparedEvent(
                                          subscription,
                                          subscriptionPlanChanging,
                                          previousSubscriptionCycleId));

            subscription.SubscriptionPlanChangeStatus = SubscriptionPlanChangeStatus.Pending;

            _dbContext.SubscriptionPlanChanges.Remove(subscriptionPlanChanging);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }
    }
}
