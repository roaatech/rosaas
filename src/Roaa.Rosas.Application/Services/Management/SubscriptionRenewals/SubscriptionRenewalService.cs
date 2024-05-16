using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals
{
    public class SubscriptionRenewalService : ISubscriptionRenewalService
    {
        #region Props 
        private readonly SubscriptionRenewalUtilities _utilities;
        private readonly ILogger<SubscriptionRenewalService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;
        #endregion


        #region Corts
        public SubscriptionRenewalService(SubscriptionRenewalUtilities utilities,
                                           ILogger<SubscriptionRenewalService> logger,
                                           IIdentityContextService identityContextService,
                                           IPublisher publisher,
                                           IRosasDbContext dbContext)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _dbContext = dbContext;
            _utilities = utilities;
            _publisher = publisher;
        }

        #endregion


        #region Services 

        public async Task<Result<List<SubscriptionRenewalDto>>> GetSubscriptionRenewalsListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var subscriptionAutoRenewals = await _dbContext.SubscriptionRenewals.AsNoTracking()
                                                        .Where(x => _identityContextService.IsSuperAdmin() ||
                                                                    _dbContext.EntityAdminPrivileges
                                                                            .Any(a => a.UserId == userId &&
                                                                                      a.EntityId == x.Subscription.TenantId &&
                                                                                      a.EntityType == EntityType.Tenant
                                                                                ))
                                                         .Select(autoRenewal => new SubscriptionRenewalDto
                                                         {
                                                             Id = autoRenewal.Id,
                                                             Comment = autoRenewal.Comment,
                                                             EnabledDate = autoRenewal.ModificationDate,
                                                             SubscriptionRenewalDate = autoRenewal.SubscriptionRenewalDate,
                                                             IsContinuousRenewal = autoRenewal.IsContinuousRenewal,
                                                             RenewalsCount = autoRenewal.RenewalsCount,
                                                             Status = autoRenewal.Status,
                                                             Type = autoRenewal.Type,
                                                             Product = new Common.Models.CustomLookupItemDto<Guid>
                                                             {
                                                                 Id = autoRenewal.Subscription.ProductId,
                                                                 SystemName = autoRenewal.Subscription.Product.SystemName,
                                                                 DisplayName = autoRenewal.Subscription.Product.DisplayName,
                                                             },
                                                             Subscription = new Common.Models.CustomLookupItemDto<Guid>
                                                             {
                                                                 Id = autoRenewal.SubscriptionId,
                                                                 SystemName = autoRenewal.Subscription.Tenant.SystemName,
                                                                 DisplayName = autoRenewal.Subscription.Tenant.DisplayName,
                                                             },
                                                             Plan = new SubscriptionRenewalDto.AutoRenewalPlanDto
                                                             {
                                                                 Id = autoRenewal.PlanId,
                                                                 DisplayName = autoRenewal.PlanDisplayName,
                                                                 PlanPriceId = autoRenewal.PlanPriceId,
                                                                 Cycle = autoRenewal.PlanCycle,
                                                                 Price = autoRenewal.Price,
                                                             },
                                                         })
                                                         .OrderByDescending(x => x.EnabledDate)
                                                         .ToListAsync(cancellationToken);

            return Result<List<SubscriptionRenewalDto>>.Successful(subscriptionAutoRenewals);
        }


        public async Task<Result> CancelRenewalAsync(Guid renewalId, Guid subscriptionId, string? comment, CancellationToken cancellationToken)
        {
            var subscriptionRenewal = await _dbContext.SubscriptionRenewals
                                            .Where(x => _identityContextService.IsSuperAdmin() ||
                                                        _dbContext.EntityAdminPrivileges
                                                                    .Any(a =>
                                                                        a.UserId == _identityContextService.UserId &&
                                                                        a.EntityId == x.Subscription.TenantId &&
                                                                        a.EntityType == EntityType.Tenant
                                                                        )
                                                    )
                                            .Where(x => x.Id == renewalId && x.SubscriptionId == subscriptionId)
                                            .SingleOrDefaultAsync(cancellationToken);
            if (subscriptionRenewal is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(subscriptionId));
            }

            if (!_utilities.EnsureAllowedRenewalCancellationStatuses(subscriptionRenewal.Status))
            {
                throw new NullReferenceException($"You cannot cancel subscription renewal({subscriptionRenewal.Type}) in {subscriptionRenewal.Status} status.");
            }

            var linkedCard = await _dbContext.LinkedCards
                                             .Where(x => x.EntityId == subscriptionRenewal.Id &&
                                                         x.EntityType == EntityType.SubscriptionRenewal)
                                             .SingleOrDefaultAsync(cancellationToken);

            if (linkedCard is not null)
            {
                _dbContext.LinkedCards.Remove(linkedCard);
            }

            _dbContext.SubscriptionRenewals.Remove(subscriptionRenewal);

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            if (result > 0) { await _publisher.Publish(new SubscriptionAutorenewalDisabledEvent(subscriptionRenewal), cancellationToken); }


            return Result.Successful();
        }


        public async Task<Result> EnableAutoRenewalAsync(Guid subscriptionId,
                                                         string cardReferenceId,
                                                         PaymentPlatform paymentPlatform,
                                                         Guid? planPriceId,
                                                         string? comment,
                                                         Guid userId,
                                                         UserType payerUsertype,
                                                         int renewalsCount,
                                                         bool isContinuousRenewal,
                                                         CancellationToken cancellationToken = default)
        {

            var customeSubscription = await _dbContext.Subscriptions
                                                 .Where(x => _identityContextService.IsSuperAdmin() ||
                                                            !_identityContextService.IsAuthenticated ||
                                                            _dbContext.EntityAdminPrivileges
                                                                    .Any(a =>
                                                                        a.UserId == _identityContextService.UserId &&
                                                                        a.EntityId == x.TenantId &&
                                                                        a.EntityType == EntityType.Tenant
                                                                        )
                                                        )
                                                 .Where(x => x.Id == subscriptionId)
                                                 .Select(x => new { x.PlanPriceId, x.SubscriptionMode, x.EndDate })
                                                 .SingleOrDefaultAsync(cancellationToken);
            if (customeSubscription is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(subscriptionId));
            }

            Guid subscriptionPlanPriceId = customeSubscription.PlanPriceId;

            if (customeSubscription.SubscriptionMode == SubscriptionMode.Trial)
            {
                var trialSubscription = await _dbContext.TrialSubscriptions
                                                 .Where(x => x.SubscriptionId == subscriptionId)
                                                 .Select(x => new { x.SelectedPlanPriceId })
                                                 .SingleOrDefaultAsync(cancellationToken);

                subscriptionPlanPriceId = trialSubscription.SelectedPlanPriceId;
            }

            subscriptionPlanPriceId = planPriceId ?? subscriptionPlanPriceId;

            var planPrice = await _dbContext.PlanPrices
                                            .Include(p => p.Plan)
                                            .Where(x => x.Id == subscriptionPlanPriceId)
                                            .SingleOrDefaultAsync();
            if (planPrice is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(planPriceId));
            }


            var date = DateTime.UtcNow;
            var subscriptionRenewal = await _dbContext.SubscriptionRenewals
                                                .Where(x => x.SubscriptionId == subscriptionId)
                                                .SingleOrDefaultAsync();
            if (subscriptionRenewal is not null)
            {
                switch (subscriptionRenewal.Type)
                {
                    case SubscriptionRenewalTypeEnum.Upgrade:
                    case SubscriptionRenewalTypeEnum.Downgrade:
                        {
                            subscriptionRenewal.IsContinuousRenewal = isContinuousRenewal;
                            subscriptionRenewal.RenewalsCount = renewalsCount <= 1 ? 0 : renewalsCount - 1;
                            subscriptionRenewal.ModificationDate = DateTime.UtcNow;
                            subscriptionRenewal.ModifiedByUserId = _identityContextService.GetActorId();
                        }
                        break;
                    case SubscriptionRenewalTypeEnum.AutoRenewal:
                    default:
                        return Result.Fail(ErrorMessage.SubscriptionAlreadyEnabledAutoRenewal, _identityContextService.Locale, nameof(subscriptionId));
                }
            }
            else
            {
                subscriptionRenewal = new SubscriptionRenewal
                {
                    Id = subscriptionId,
                    SubscriptionId = subscriptionId,
                    PlanPriceId = planPrice.Id,
                    PlanId = planPrice.PlanId,
                    PlanCycle = planPrice.PlanCycle,
                    Price = planPrice.Price,
                    PlanDisplayName = planPrice.Plan.DisplayName,
                    Status = SubscriptionRenewalStatus.None,
                    IsContinuousRenewal = isContinuousRenewal,
                    RenewalsCount = renewalsCount <= 1 ? 0 : renewalsCount - 1,
                    SubscriptionRenewalDate = customeSubscription.EndDate.Value, // TODO : currently renewal for plans of planned tenancy type,
                    Comment = comment,
                    CreatedByUserId = userId,
                    ModifiedByUserId = userId,
                    CreatedByUserType = payerUsertype,
                    CreationDate = date,
                    ModificationDate = date,
                    Type = SubscriptionRenewalTypeEnum.AutoRenewal,
                };

                _dbContext.SubscriptionRenewals.Add(subscriptionRenewal);

                var linkedCard = new LinkedCard
                {
                    Id = Guid.NewGuid(),
                    ReferenceId = cardReferenceId,
                    PaymentPlatform = paymentPlatform,
                    EntityId = subscriptionRenewal.Id,
                    EntityType = EntityType.SubscriptionRenewal,
                };

                _dbContext.LinkedCards.Add(linkedCard);
            }

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        await _publisher.Publish(new SubscriptionAutorenewalEnabledEvent(subscriptionRenewal), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occurred during publishing: {ex.Message}");
                    }
                });
            }
            return Result.Successful();
        }

        public async Task<Result> EnableSubscriptionUpgradingAsync(Guid subscriptionId,
                                                        Guid planId,
                                                        Guid planPriceId,
                                                        string cardReferenceId,
                                                        PaymentPlatform paymentPlatform,
                                                        string? comment,
                                                        CancellationToken cancellationToken = default)
        {
            return await EnableSubscriptionRenewalAsync(subscriptionId,
                                                        planId,
                                                        planPriceId,
                                                        isForced: false,
                                                        cardReferenceId,
                                                        paymentPlatform,
                                                        comment,
                                                        x => x.SubscriptionUpgradeUrl,
                                                        SubscriptionRenewalTypeEnum.Upgrade,
                                                        subscription: null,
                                                        cancellationToken);
        }

        public async Task<Result> EnableSubscriptionDowngradingAsync(Guid subscriptionId,
                                                        Guid planId,
                                                        Guid planPriceId,
                                                        string cardReferenceId,
                                                        PaymentPlatform paymentPlatform,
                                                        string? comment,
                                                        CancellationToken cancellationToken = default)
        {
            return await EnableSubscriptionRenewalAsync(subscriptionId,
                                                        planId,
                                                        planPriceId,
                                                        isForced: false,
                                                        cardReferenceId,
                                                        paymentPlatform,
                                                        comment,
                                                        x => x.SubscriptionDowngradeUrl,
                                                        SubscriptionRenewalTypeEnum.Downgrade,
                                                        subscription: null,
                                                        cancellationToken);
        }
        public async Task<Result> EnableSubscriptionDowngradingAsync(
                                                        Subscription subscription,
                                                        Guid planId,
                                                        Guid planPriceId,
                                                        string? comment,
                                                        CancellationToken cancellationToken = default)
        {
            return await EnableSubscriptionRenewalAsync(subscriptionId: subscription.Id,
                                                        planId: planId,
                                                        planPriceId: planPriceId,
                                                        isForced: true,
                                                        cardReferenceId: null,
                                                        paymentPlatform: null,
                                                        comment: comment,
                                                        urlSelector: x => x.SubscriptionDowngradeUrl,
                                                        renewalType: SubscriptionRenewalTypeEnum.Downgrade,
                                                        subscription: subscription,
                                                        cancellationToken: cancellationToken);
        }


        private async Task<Result> EnableSubscriptionRenewalAsync(Guid subscriptionId,
                                                                    Guid planId,
                                                                    Guid planPriceId,
                                                                    bool isForced,
                                                                    string? cardReferenceId,
                                                                    PaymentPlatform? paymentPlatform,
                                                                    string? comment,
                                                                    Expression<Func<Product, string?>> urlSelector,
                                                                    SubscriptionRenewalTypeEnum renewalType,
                                                                    Subscription? subscription = null,
                                                                    CancellationToken cancellationToken = default)
        {
            if (await _dbContext.SubscriptionRenewals
                                  .Where(x => x.SubscriptionId == subscriptionId)
                                  .AnyAsync(cancellationToken))
            {
                return Result.Fail(ErrorMessage.SubscriptionAlreadyUpgradedDowngraded, _identityContextService.Locale, nameof(subscriptionId));
            }

            subscription = subscription ?? await _dbContext.Subscriptions
                                              .Where(x => _identityContextService.IsSuperAdmin() ||
                                                          _dbContext.EntityAdminPrivileges
                                                                  .Any(a =>
                                                                      a.UserId == _identityContextService.UserId &&
                                                                      a.EntityId == x.TenantId &&
                                                                      a.EntityType == EntityType.Tenant
                                                                      )
                                                      )
                                            .Where(x => x.Id == subscriptionId)
                                            .SingleOrDefaultAsync(cancellationToken);

            if (subscription is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(subscriptionId));
            }

            var url = await _dbContext.Products.AsNoTracking()
                                         .Where(x => x.Id == subscription.ProductId)
                                         .Select(urlSelector)
                                         .SingleOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(url))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }

            var planPrice = await _dbContext.PlanPrices
                                            .Where(x => x.Id == planPriceId && x.PlanId == planId)
                                            .Include(x => x.Plan)
                                            .SingleOrDefaultAsync();
            if (planPrice is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(planPriceId));
            }

            if (subscription.ProductId != planPrice.Plan.ProductId)
            {
                return Result.Fail(ErrorMessage.PlanDoesNotBelongToProduct, _identityContextService.Locale, nameof(planId));
            }

            var date = DateTime.UtcNow;

            var subscriptionRenewal = new SubscriptionRenewal
            {
                Id = isForced ? Guid.NewGuid() : subscriptionId,
                SubscriptionId = subscriptionId,
                PlanPriceId = planPrice.Id,
                PlanId = planPrice.PlanId,
                PlanCycle = planPrice.PlanCycle,
                Price = planPrice.Price,
                PlanDisplayName = planPrice.Plan.DisplayName ?? "",
                Status = SubscriptionRenewalStatus.None,
                SubscriptionRenewalDate = subscription.EndDate.Value, // TODO : currently renewal for plans of planned tenancy type,
                Comment = comment,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreatedByUserType = _identityContextService.GetUserType(),
                CreationDate = date,
                ModificationDate = date,
                Type = renewalType,
                IsForced = isForced,
            };

            _dbContext.SubscriptionRenewals.Add(subscriptionRenewal);

            if (!_utilities.EnsureIsForcedDowngrade(subscriptionRenewal))
            {
                ArgumentNullException.ThrowIfNull(cardReferenceId);
                ArgumentNullException.ThrowIfNull(paymentPlatform);

                var linkedCard = new LinkedCard
                {
                    Id = Guid.NewGuid(),
                    ReferenceId = cardReferenceId,
                    PaymentPlatform = paymentPlatform.Value,
                    EntityId = subscriptionRenewal.Id,
                    EntityType = EntityType.SubscriptionRenewal,
                };

                _dbContext.LinkedCards.Add(linkedCard);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }
        #endregion

    }
}
