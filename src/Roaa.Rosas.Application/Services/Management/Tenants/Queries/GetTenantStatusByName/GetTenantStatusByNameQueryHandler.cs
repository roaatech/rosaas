using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantStatusByName
{
    public class GetTenantStatusByNameQueryHandler : IRequestHandler<GetTenantStatusByNameQuery, Result<TenantStatusDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetTenantStatusByNameQueryHandler(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<TenantStatusDto>> Handle(GetTenantStatusByNameQuery request, CancellationToken cancellationToken)
        {
            var tenantStatus = await _dbContext.ProductTenants.AsNoTracking()
                                                 .Where(x => x.ProductId == request.ProductId &&
                                                         request.TenantName.ToLower().Equals(x.Tenant.UniqueName))
                                                  .Select(x => new TenantStatusDto
                                                  {
                                                      Status = x.Status,
                                                      IsActive = x.Status == TenantStatus.Active,
                                                  })
                                                  .SingleOrDefaultAsync(cancellationToken);

            return Result<TenantStatusDto>.Successful(tenantStatus);
        }
        #endregion
    }
}
