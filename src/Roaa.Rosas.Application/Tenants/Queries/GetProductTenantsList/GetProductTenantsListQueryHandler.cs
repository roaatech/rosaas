using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetProductTenantsList
{
    public class GetProductTenantsListQueryHandler : IRequestHandler<GetProductTenantsListQuery, Result<List<ProductTenantListItemDto>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantWorkflow _workflow;
        #endregion


        #region Corts
        public GetProductTenantsListQueryHandler(
            IRosasDbContext dbContext,
        ITenantWorkflow workflow,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _workflow = workflow;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<List<ProductTenantListItemDto>>> Handle(GetProductTenantsListQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.ProductTenants.AsNoTracking()
                                                 .Where(x => x.ProductId == request.ProductId)
                                                 .Select(x => new ProductTenantListItemDto
                                                 {
                                                     Id = x.Tenant.Id,
                                                     UniqueName = x.Tenant.UniqueName,
                                                     Title = x.Tenant.Title,
                                                     Status = x.Status,
                                                     CreatedDate = x.Tenant.Created,
                                                     EditedDate = x.Tenant.Edited,
                                                 })
                                                 .ToListAsync(cancellationToken);

            return Result<List<ProductTenantListItemDto>>.Successful(tenants);
        }
        #endregion
    }
}
