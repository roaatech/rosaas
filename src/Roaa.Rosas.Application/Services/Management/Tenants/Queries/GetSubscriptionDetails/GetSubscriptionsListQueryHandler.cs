using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsList
{
    public class GetSubscriptionDetailsQueryHandler : IRequestHandler<GetSubscriptionDetailsQuery, Result<SubscriptionDetailsDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public GetSubscriptionDetailsQueryHandler(IRosasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion


        #region Handler   
        public async Task<Result<SubscriptionDetailsDto>> Handle(GetSubscriptionDetailsQuery request, CancellationToken cancellationToken)
        {
            var subscription = await _dbContext.Subscriptions.AsNoTracking()
                                                 .Where(x => x.TenantId == request.TenantId &&
                                                             x.ProductId == request.ProductId)
                                                 .Select(subscription => new SubscriptionDetailsDto
                                                 {
                                                     SubscriptionId = subscription.Id,
                                                     Plan = new Common.Models.LookupItemDto<Guid>
                                                     {
                                                         Id = subscription.Plan.Id,
                                                         Name = subscription.Plan.Name,
                                                     },
                                                     PlanPrice = new PlanPriceDto
                                                     {
                                                         Id = subscription.PlanPrice.Id,
                                                         Cycle = subscription.PlanPrice.Cycle,
                                                         Price = subscription.PlanPrice.Price,
                                                     },
                                                     SubscriptionFeatures = subscription.SubscriptionFeatures.Select(x => new SubscriptionFeatureDto
                                                     {
                                                         EndDate = x.EndDate,
                                                         StartDate = x.StartDate,
                                                         RemainingUsage = x.RemainingUsage,
                                                         Feature = new FeatureDto
                                                         {
                                                             Id = x.Feature.Id,
                                                             Name = x.Feature.Name,
                                                             Description = x.Feature.Description,
                                                             Type = x.Feature.Type,
                                                             Reset = x.Feature.Reset,
                                                             Limit = x.PlanFeature.Limit,
                                                             Unit = x.PlanFeature.Unit,
                                                         }
                                                     })
                                                 })
                                                 .SingleOrDefaultAsync(cancellationToken);

            return Result<SubscriptionDetailsDto>.Successful(subscription);
        }
        #endregion
    }
}
