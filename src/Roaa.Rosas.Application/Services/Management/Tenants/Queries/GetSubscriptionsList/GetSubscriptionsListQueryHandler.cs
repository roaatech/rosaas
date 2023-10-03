using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsList
{
    public class GetSubscriptionsListQueryHandler : IRequestHandler<GetSubscriptionsListQuery, Result<List<SubscriptionListItemDto>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public GetSubscriptionsListQueryHandler(IRosasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion


        #region Handler   
        public async Task<Result<List<SubscriptionListItemDto>>> Handle(GetSubscriptionsListQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.Subscriptions.AsNoTracking()
                                                 .Where(x => x.ProductId == request.ProductId)
                                                 .Select(x => new SubscriptionListItemDto
                                                 {
                                                     SubscriptionId = x.Id,
                                                     Id = x.Tenant.Id,
                                                     TenantId = x.Tenant.Id,
                                                     UniqueName = x.Tenant.UniqueName,
                                                     HealthCheckUrl = x.HealthCheckUrl,
                                                     HealthCheckUrlIsOverridden = x.HealthCheckUrlIsOverridden,
                                                     Title = x.Tenant.Title,
                                                     Status = x.Status,
                                                     IsPaid = x.IsPaid,
                                                     EndDate = x.EndDate,
                                                     StartDate = x.StartDate,
                                                     Plan = new Common.Models.LookupItemDto<Guid>(x.PlanId, x.Plan.Name),
                                                     CreatedDate = x.Tenant.CreationDate,
                                                     EditedDate = x.Tenant.ModificationDate,
                                                 })
                                                 .ToListAsync(cancellationToken);

            return Result<List<SubscriptionListItemDto>>.Successful(tenants);
        }
        #endregion
    }
}
