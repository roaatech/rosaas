using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenentByNameAndProductId
{
    public class GetTenentByNameAndProductIdQueryHandler : IRequestHandler<GetTenentByNameAndProductIdQuery, Result<ProductTenantDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetTenentByNameAndProductIdQueryHandler(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<ProductTenantDto>> Handle(GetTenentByNameAndProductIdQuery request, CancellationToken cancellationToken)
        {
            var tenant = await _dbContext.Subscriptions
                                         .AsNoTracking()
                                         .Where(x => x.ProductId == request.ProductId &&
                                                request.TenantName.ToLower().Equals(x.Tenant.UniqueName))
                                         .Select(subscription => new ProductTenantDto
                                         {
                                             UniqueName = subscription.Tenant.UniqueName,
                                             HealthCheckUrl = subscription.HealthCheckUrl,
                                             HealthCheckUrlIsOverridden = subscription.HealthCheckUrlIsOverridden,
                                             Title = subscription.Tenant.Title,
                                             Status = subscription.Status,
                                             Step = subscription.Step,
                                             IsPaid = subscription.IsPaid,
                                             EndDate = subscription.EndDate,
                                             StartDate = subscription.StartDate,
                                             LastResetDate = subscription.LastResetDate,
                                             LastLimitsResetDate = subscription.LastLimitsResetDate,
                                             Plan = new Common.Models.LookupItemDto<Guid>(subscription.PlanId, subscription.Plan.Name),
                                             CreatedDate = subscription.Tenant.CreationDate,
                                             EditedDate = subscription.Tenant.ModificationDate,
                                             Metadata = subscription.Metadata,
                                             Specifications = subscription.SpecificationsValues.Select(specVal => new SpecificationListItemDto
                                             {
                                                 DisplayName = specVal.Specification.DisplayName,
                                                 Description = specVal.Specification.Description,
                                                 Name = specVal.Specification.Name,
                                                 IsRequired = specVal.Specification.IsRequired,
                                                 IsUserEditable = specVal.Specification.IsUserEditable,
                                                 ValidationFailureDescription = specVal.Specification.ValidationFailureDescription,
                                                 RegularExpression = specVal.Specification.RegularExpression,
                                                 Value = specVal.Value,
                                             }),
                                         })
                                         .SingleOrDefaultAsync(cancellationToken);

            return Result<ProductTenantDto>.Successful(tenant);
        }

        #endregion
    }
}
