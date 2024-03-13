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

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.PlansChanging
{
    public class SubscriptionPlanChangingService : ISubscriptionPlanChangingService
    {
        #region Props 
        private readonly ILogger<SubscriptionPlanChangingService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public SubscriptionPlanChangingService(ILogger<SubscriptionPlanChangingService> logger,
                                   IIdentityContextService identityContextService,
                                   IRosasDbContext dbContext)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _dbContext = dbContext;
        }

        #endregion


        #region Services 

        public async Task<Result> CreateSubscriptionUpgradeAsync(Guid subscriptionId,
                                                                Guid planId,
                                                                Guid planPriceId,
                                                                string cardReferenceId,
                                                                PaymentPlatform paymentPlatform,
                                                                string? comment,
                                                                CancellationToken cancellationToken = default)
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
                                              .Where(x => x.Id == subscriptionId)
                                              .SingleOrDefaultAsync();

            if (subscription is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(subscriptionId));
            }


            if (await _dbContext.SubscriptionPlanChanges
                                .Where(x => x.Id == subscriptionId)
                                .AnyAsync())
            {
                return Result.Fail(ErrorMessage.SubscriptionAlreadyUpgradedDowngraded, _identityContextService.Locale, nameof(subscriptionId));
            }

            var upgradeUrl = await _dbContext.Products.AsNoTracking()
                                              .Where(x => x.Id == subscription.ProductId)
                                              .Select(x => x.SubscriptionUpgradeUrl)
                                              .SingleOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(upgradeUrl))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }

            var plan = await _dbContext.Plans
                                        .Where(x => x.Id == planId)
                                        .SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(planId));
            }

            var planPrice = await _dbContext.PlanPrices
                                            .Where(x => x.Id == planPriceId &&
                                                        x.PlanId == planId)

                                            .SingleOrDefaultAsync();
            if (planPrice is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(planPriceId));
            }

            if (subscription.ProductId != plan.ProductId)
            {
                return Result.Fail(ErrorMessage.PlanDoesNotBelongToProduct, _identityContextService.Locale, nameof(planId));
            }

            var date = DateTime.UtcNow;

            var subscriptionPlanChanging = new SubscriptionPlanChanging
            {
                Id = subscriptionId,
                Type = PlanChangingType.Upgrade,
                SubscriptionId = subscriptionId,
                PlanPriceId = planPriceId,
                PlanId = planPrice.PlanId,
                PlanCycle = planPrice.PlanCycle,
                Price = planPrice.Price,
                PlanDisplayName = plan.DisplayName ?? "",
                IsPaid = true,
                Comment = comment,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            subscription.SubscriptionPlanChangeStatus = null;

            subscription.AddDomainEvent(new SubscriptionUpgradeRequestedEvent(subscription));

            _dbContext.SubscriptionPlanChanges.Add(subscriptionPlanChanging);


            var linkedCard = new LinkedCard
            {
                Id = Guid.NewGuid(),
                ReferenceId = cardReferenceId,
                PaymentPlatform = paymentPlatform,
                EntityId = subscriptionPlanChanging.Id,
                EntityType = EntityType.SubscriptionPlanChanging,
            };

            _dbContext.LinkedCards.Add(linkedCard);


            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public async Task<Result> CreateSubscriptionDowngradeAsync(Guid subscriptionId,
                                                                    Guid planId,
                                                                    Guid planPriceId,
                                                                    string cardReferenceId,
                                                                    PaymentPlatform paymentPlatform,
                                                                    string? comment,
                                                                    CancellationToken cancellationToken = default)
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
                                                .Where(x => x.Id == subscriptionId)
                                                .SingleOrDefaultAsync();

            if (subscription is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(subscriptionId));
            }


            if (await _dbContext.SubscriptionPlanChanges
                                        .Where(x => x.Id == subscriptionId)
                                        .AnyAsync())
            {
                return Result.Fail(ErrorMessage.SubscriptionAlreadyUpgradedDowngraded, _identityContextService.Locale, nameof(subscriptionId));
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
                                        .Where(x => x.Id == planId)
                                        .SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(planId));
            }

            var planPrice = await _dbContext.PlanPrices
                                            .Where(x => x.Id == planPriceId &&
                                                        x.PlanId == planId)

                                            .SingleOrDefaultAsync();
            if (planPrice is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(planPriceId));
            }

            if (subscription.ProductId != plan.ProductId)
            {
                return Result.Fail(ErrorMessage.PlanDoesNotBelongToProduct, _identityContextService.Locale, nameof(planId));
            }

            var date = DateTime.UtcNow;

            var subscriptionPlanChanging = new SubscriptionPlanChanging
            {
                Id = subscriptionId,
                Type = PlanChangingType.Downgrade,
                SubscriptionId = subscriptionId,
                PlanPriceId = planPriceId,
                PlanId = planPrice.PlanId,
                PlanCycle = planPrice.PlanCycle,
                Price = planPrice.Price,
                PlanDisplayName = plan.DisplayName ?? "",
                IsPaid = true,
                Comment = comment,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            subscription.SubscriptionPlanChangeStatus = null;

            subscription.AddDomainEvent(new SubscriptionDowngradeRequestedEvent(subscription));

            _dbContext.SubscriptionPlanChanges.Add(subscriptionPlanChanging);

            var linkedCard = new LinkedCard
            {
                Id = Guid.NewGuid(),
                ReferenceId = cardReferenceId,
                PaymentPlatform = paymentPlatform,
                EntityId = subscriptionPlanChanging.Id,
                EntityType = EntityType.SubscriptionPlanChanging,
            };

            _dbContext.LinkedCards.Add(linkedCard);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        #endregion

    }
}
