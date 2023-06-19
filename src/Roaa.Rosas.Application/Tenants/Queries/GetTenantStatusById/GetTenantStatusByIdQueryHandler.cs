using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantStatusById
{
    public class GetTenantStatusByIdQueryHandler : IRequestHandler<GetTenantStatusByIdQuery, Result<TenantStatusDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetTenantStatusByIdQueryHandler(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<TenantStatusDto>> Handle(GetTenantStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantStatus = await _dbContext.ProductTenants.AsNoTracking()
                                                 .Include(x => x.Tenant)
                                                  .Where(x => x.TenantId == request.Id && x.ProductId == request.ProductId)
                                                  .Select(x => new TenantStatusDto
                                                  {
                                                      Status = x.Tenant.Status,
                                                      IsActive = x.Tenant.Status == TenantStatus.Active,
                                                  })
                                                  .SingleOrDefaultAsync(cancellationToken);

            return Result<TenantStatusDto>.Successful(tenantStatus);
        }
        #endregion
    }
}
