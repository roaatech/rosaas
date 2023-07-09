using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantMetadataById
{
    public class GetTenantMetadataByIdQueryHandler : IRequestHandler<GetTenantMetadataByIdQuery, Result<Dictionary<string, string>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetTenantMetadataByIdQueryHandler(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<Dictionary<string, string>>> Handle(GetTenantMetadataByIdQuery request, CancellationToken cancellationToken)
        {
            var metadata = await _dbContext.ProductTenants.AsNoTracking()
                                                 .Include(x => x.Tenant)
                                                  .Where(x => x.TenantId == request.TenantId && x.ProductId == request.ProductId)
                                                  .Select(x => x.Metadata)
                                                  .SingleOrDefaultAsync(cancellationToken);

            return Result<Dictionary<string, string>>.Successful(metadata);
        }
        #endregion
    }
}
