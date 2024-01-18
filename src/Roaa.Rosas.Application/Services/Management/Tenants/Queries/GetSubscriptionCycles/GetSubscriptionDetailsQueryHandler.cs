using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionCycles
{
    public class GetSubscriptionCyclesQueryHandler : IRequestHandler<GetSubscriptionCyclesQuery, Result<List<SubscriptionCycleDto>>>
    {

        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetSubscriptionCyclesQueryHandler(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }
        #endregion


        #region Handler    
        public async Task<Result<List<SubscriptionCycleDto>>> Handle(GetSubscriptionCyclesQuery request, CancellationToken cancellationToken)
        {
            var subscriptionCycles = await _dbContext.SubscriptionCycles
                                                .AsNoTracking()
                                                .Where(x => _identityContextService.IsSuperAdmin() ||
                                                            _dbContext.EntityAdminPrivileges
                                                .Any(a =>
                                                                            a.UserId == _identityContextService.UserId &&
                                                                            a.EntityId == x.Id &&
                                                                            a.EntityType == EntityType.Tenant
                                                                            )
                                                        )
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
                                                                 CycleType = SubscriptionCycle.Type,
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

/*
    var subscriptionCycles = await _dbContext.SubscriptionCycles.AsNoTracking()
                                                                          .Join(_dbContext.TenantAdmins,
                                                                  cycle => cycle.TenantId,
                                                                  admin => admin.TenantId,
                                                                  (cycle, admin) => new { cycle, admin })
                                                            .Where(result => result.admin.UserId == _identityContextService.UserId &&
                                                                             result.admin.UserType == _identityContextService.GetUserType() &&
                                                                             result.cycle.SubscriptionId == request.SubscriptionId &&
                                                                            (request.SubscriptionCycleId == null ||
                                                                             result.cycle.Id == request.SubscriptionCycleId))
*/