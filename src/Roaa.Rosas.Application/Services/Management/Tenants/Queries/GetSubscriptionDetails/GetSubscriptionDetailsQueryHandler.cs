using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionDetails
{
    public class GetSubscriptionDetailsQueryHandler : IRequestHandler<GetSubscriptionDetailsQuery, Result<SubscriptionDetailsDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetSubscriptionDetailsQueryHandler(IRosasDbContext dbContext,
                                                  IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }
        #endregion


        #region Handler   
        public async Task<Result<SubscriptionDetailsDto>> Handle(GetSubscriptionDetailsQuery request, CancellationToken cancellationToken)
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
                                                .Where(x => x.TenantId == request.TenantId &&
                                                             x.ProductId == request.ProductId)
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
                                                     SubscriptionPlanChangeStatus = subscription.SubscriptionPlanChangeStatus,
                                                     IsActive = subscription.IsActive,
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

            if (subscription is null)
            {
                return Result<SubscriptionDetailsDto>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            //subscription.HasSubscriptionFeaturesLimitsResettable = subscription.SubscriptionFeatures
            //                                                                    .Select(x => x.Feature.Reset)
            //                                                                    .Where(reset => FeatureResetManager.FromKey(reset).IsResettable())
            //                                                                    .Any();

            subscription.IsPlanChangeAllowed = subscription.SubscriptionPlanChange is null &&
                                                (subscription.SubscriptionPlanChangeStatus is null ||
                                                 subscription.SubscriptionPlanChangeStatus == SubscriptionPlanChangeStatus.Done) &&
                                                 subscription.IsSubscriptionUpgradeUrlExists &&
                                                 subscription.IsSubscriptionDowngradeUrlExists;

            subscription.IsResettableAllowed = (subscription.LastResetDate is null || DateTime.UtcNow > subscription.LastResetDate.Value.AddHours(24)) &&
                                               (subscription.SubscriptionResetStatus is null ||
                                                subscription.SubscriptionResetStatus == SubscriptionResetStatus.Done) &&
                                                subscription.IsSubscriptionResetUrlExists;

            return Result<SubscriptionDetailsDto>.Successful(subscription);
        }
        #endregion
    }
}
