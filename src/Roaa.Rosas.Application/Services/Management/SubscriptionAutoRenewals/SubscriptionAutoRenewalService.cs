using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.SubscriptionAutoRenewals.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionAutoRenewals
{
    public class SubscriptionAutoRenewalService : ISubscriptionAutoRenewalService
    {
        #region Props 
        private readonly ILogger<SubscriptionAutoRenewalService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public SubscriptionAutoRenewalService(ILogger<SubscriptionAutoRenewalService> logger,
                                   IIdentityContextService identityContextService,
                                   IRosasDbContext dbContext)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _dbContext = dbContext;
        }

        #endregion


        #region Services 

        public async Task<Result<List<SubscriptionAutoRenewalDto>>> GetSubscriptionAutoRenewalsListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var subscriptionAutoRenewals = await _dbContext.SubscriptionAutoRenewals.AsNoTracking()
                                                        .Where(x => _identityContextService.IsSuperAdmin() ||
                                                                    _dbContext.EntityAdminPrivileges
                                                                            .Any(a => a.UserId == userId &&
                                                                                      a.EntityId == x.Subscription.TenantId &&
                                                                                      a.EntityType == EntityType.Tenant
                                                                                ))
                                                         .Select(autoRenewal => new SubscriptionAutoRenewalDto
                                                         {
                                                             Id = autoRenewal.Id,
                                                             Product = new Common.Models.CustomLookupItemDto<Guid>(autoRenewal.Subscription.ProductId, autoRenewal.Subscription.Product.SystemName, autoRenewal.Subscription.Product.DisplayName),
                                                             Subscription = new Common.Models.CustomLookupItemDto<Guid>(autoRenewal.SubscriptionId, autoRenewal.Subscription.Tenant.SystemName, autoRenewal.Subscription.Tenant.DisplayName),
                                                             Plan = new SubscriptionAutoRenewalDto.AutoRenewalPlanDto
                                                             {
                                                                 Id = autoRenewal.PlanId,
                                                                 DisplayName = autoRenewal.PlanDisplayName,
                                                                 PlanPriceId = autoRenewal.PlanPriceId,
                                                                 Cycle = autoRenewal.PlanCycle,
                                                                 Price = autoRenewal.Price,
                                                             },
                                                             Comment = autoRenewal.Comment,
                                                             EnabledDate = autoRenewal.ModificationDate,
                                                             SubscriptionRenewalDate = autoRenewal.Subscription.EndDate,
                                                         })
                                                         .OrderByDescending(x => x.EnabledDate)
                                                         .ToListAsync(cancellationToken);

            return Result<List<SubscriptionAutoRenewalDto>>.Successful(subscriptionAutoRenewals);
        }
        public async Task<Result> CancelAutoRenewalAsync(Guid subscriptionId, string? comment, CancellationToken cancellationToken)
        {
            var autoRenewal = await _dbContext.SubscriptionAutoRenewals
                                    .Where(x => _identityContextService.IsSuperAdmin() ||
                                                _dbContext.EntityAdminPrivileges
                                                            .Any(a =>
                                                                a.UserId == _identityContextService.UserId &&
                                                                a.EntityId == x.Subscription.TenantId &&
                                                                a.EntityType == EntityType.Tenant
                                                                )
                                            )
                                    .Where(x => x.Id == subscriptionId)
                                      .SingleOrDefaultAsync(cancellationToken);
            if (autoRenewal is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(subscriptionId));
            }
            var linkedCard = await _dbContext.LinkedCards
                        .Where(x => x.EntityId == autoRenewal.Id &&
                                    x.EntityType == EntityType.SubscriptionAutoRenewal)
                        .SingleOrDefaultAsync(cancellationToken);

            if (linkedCard is not null)
            {
                _dbContext.LinkedCards.Remove(linkedCard);
            }

            autoRenewal.AddDomainEvent(new SubscriptionAutoRenewalCanceledEvent(autoRenewal, comment));

            _dbContext.SubscriptionAutoRenewals.Remove(autoRenewal);


            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public async Task<Result> EnableAutoRenewalAsync(Guid subscriptionId,
                                                         string cardReferenceId,
                                                         PaymentPlatform paymentPlatform,
                                                         Guid? planPriceId,
                                                         string? comment,
                                                         Guid userId,
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
                                                 .Select(x => new { x.PlanPriceId, x.SubscriptionMode })
                                                 .SingleOrDefaultAsync(cancellationToken);
            if (customeSubscription is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(subscriptionId));
            }

            Guid subscriptionPlanPriceId = customeSubscription.PlanPriceId;

            if (customeSubscription.SubscriptionMode == SubscriptionMode.Trial)
            {
                var trialSubscription = await _dbContext.SubscriptionTrialPeriods
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
            var autoRenewal = await _dbContext.SubscriptionAutoRenewals
                                                .Where(x => x.Id == subscriptionId)
                                                .SingleOrDefaultAsync();
            if (autoRenewal is null)
            {
                autoRenewal = new SubscriptionAutoRenewal
                {
                    Id = subscriptionId,
                    SubscriptionId = subscriptionId,
                    PlanPriceId = subscriptionPlanPriceId,
                    PlanId = planPrice.PlanId,
                    PlanCycle = planPrice.PlanCycle,
                    Price = planPrice.Price,
                    PlanDisplayName = planPrice.Plan.DisplayName,
                    UpcomingAutoRenewalsCount = 1,
                    IsPaid = true,
                    Comment = comment,
                    CreatedByUserId = userId,
                    ModifiedByUserId = userId,
                    CreationDate = date,
                    ModificationDate = date,
                };

                _dbContext.SubscriptionAutoRenewals.Add(autoRenewal);
            }
            else
            {
                autoRenewal.PlanPriceId = subscriptionPlanPriceId;
                autoRenewal.PlanId = planPrice.PlanId;
                autoRenewal.PlanCycle = planPrice.PlanCycle;
                autoRenewal.Price = planPrice.Price;
                autoRenewal.PlanDisplayName = planPrice.Plan.DisplayName;
                autoRenewal.Comment = comment;
                autoRenewal.ModifiedByUserId = userId;
                autoRenewal.ModificationDate = date;
            }


            var linkedCard = new LinkedCard
            {
                Id = Guid.NewGuid(),
                ReferenceId = cardReferenceId,
                PaymentPlatform = paymentPlatform,
                EntityId = autoRenewal.Id,
                EntityType = EntityType.SubscriptionAutoRenewal,
            };

            _dbContext.LinkedCards.Add(linkedCard);

            autoRenewal.AddDomainEvent(new SubscriptionAutoRenewalEnabledEvent(autoRenewal, comment));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        #endregion

    }
}
