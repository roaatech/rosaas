using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Subscriptions.AutoRenewals;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
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

        public async Task<Result> EnableAutoRenewalAsync(Guid subscriptionId, Guid planPriceId, string comment, CancellationToken cancellationToken = default)
        {

            if (!await _dbContext.Subscriptions
                                 .Where(x => _identityContextService.IsSuperAdmin() ||
                                            _dbContext.EntityAdminPrivileges
                                                    .Any(a =>
                                                        a.UserId == _identityContextService.UserId &&
                                                        a.EntityId == x.TenantId &&
                                                        a.EntityType == EntityType.Tenant
                                                        )
                                        )
                                 .Where(x => x.Id == subscriptionId)
                                 .AnyAsync())
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(subscriptionId));
            }


            var planPrice = await _dbContext.PlanPrices
                                            .Include(p => p.Plan)
                                            .Where(x => x.Id == planPriceId)
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
                    PlanPriceId = planPriceId,
                    PlanId = planPrice.PlanId,
                    PlanCycle = planPrice.PlanCycle,
                    Price = planPrice.Price,
                    PlanDisplayName = planPrice.Plan.DisplayName,
                    UpcomingAutoRenewalsCount = 1,
                    IsPaid = true,
                    Comment = comment,
                    CreatedByUserId = _identityContextService.UserId,
                    ModifiedByUserId = _identityContextService.UserId,
                    CreationDate = date,
                    ModificationDate = date,
                };

                _dbContext.SubscriptionAutoRenewals.Add(autoRenewal);
            }
            else
            {
                autoRenewal.PlanPriceId = planPriceId;
                autoRenewal.PlanId = planPrice.PlanId;
                autoRenewal.PlanCycle = planPrice.PlanCycle;
                autoRenewal.Price = planPrice.Price;
                autoRenewal.PlanDisplayName = planPrice.Plan.DisplayName;
                autoRenewal.Comment = comment;
                autoRenewal.ModifiedByUserId = _identityContextService.UserId;
                autoRenewal.ModificationDate = date;
            }

            autoRenewal.AddDomainEvent(new SubscriptionAutoRenewalEnabledEvent(autoRenewal, comment));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        #endregion

    }
}
