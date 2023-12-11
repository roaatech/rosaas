using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsList
{
    public class GetSubscriptionsListQueryHandler : IRequestHandler<GetSubscriptionsListQuery, Result<List<SubscriptionListItemDto>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetSubscriptionsListQueryHandler(IRosasDbContext dbContext,
                                                  IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }
        #endregion


        #region Handler   
        public async Task<Result<List<SubscriptionListItemDto>>> Handle(GetSubscriptionsListQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.Subscriptions.AsNoTracking()
                                                 //.Where(x => _identityContextService.GetUserType() == Common.Enums.UserType.SuperAdmin ||
                                                 //             _dbContext.ProductAdmins.Any(a => a.UserId == _identityContextService.UserId &&
                                                 //                                              a.ProductId == x.ProductId
                                                 //                                        )
                                                 //      )
                                                 .Where(x => x.ProductId == request.ProductId)
                                                 .Select(x => new SubscriptionListItemDto
                                                 {
                                                     SubscriptionId = x.Id,
                                                     TenantId = x.TenantId,
                                                     UniqueName = x.Tenant.UniqueName,
                                                     HealthCheckUrl = x.HealthCheckUrl,
                                                     HealthCheckUrlIsOverridden = x.HealthCheckUrlIsOverridden,
                                                     Title = x.Tenant.DisplayName,
                                                     Status = x.Status,
                                                     IsActive = x.IsActive,
                                                     EndDate = x.EndDate,
                                                     StartDate = x.StartDate,
                                                     Plan = new Common.Models.CustomLookupItemDto<Guid>(x.PlanId, x.Plan.Name, x.Plan.DisplayName),
                                                     CreatedDate = x.Tenant.CreationDate,
                                                     EditedDate = x.Tenant.ModificationDate,
                                                 })
                                                 .ToListAsync(cancellationToken);

            return Result<List<SubscriptionListItemDto>>.Successful(tenants);
        }
        #endregion
    }
}
