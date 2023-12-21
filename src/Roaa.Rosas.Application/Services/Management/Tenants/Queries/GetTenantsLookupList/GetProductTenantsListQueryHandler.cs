using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsLookupList
{
    public class GetTenantsLookupListAsyncQueryHandler : IRequestHandler<GetTenantsLookupListQuery, Result<List<LookupItemDto<Guid>>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetTenantsLookupListAsyncQueryHandler(IRosasDbContext dbContext,
                                                  IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }
        #endregion



        #region Handler   
        public async Task<Result<List<LookupItemDto<Guid>>>> Handle(GetTenantsLookupListQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.Tenants
                                        .AsNoTracking()
                                        .Where(x => _identityContextService.IsSuperAdmin() ||
                                                    _dbContext.EntityAdminPrivileges
                                                                .Any(a =>
                                                                    a.UserId == _identityContextService.UserId &&
                                                                    a.EntityId == x.Id &&
                                                                    a.EntityType == EntityType.Tenant
                                                                    )
                                                )
                                              .Select(x => new LookupItemDto<Guid>
                                              {
                                                  Id = x.Id,
                                                  SystemName = x.SystemName,
                                              })
                                              .ToListAsync(cancellationToken);

            return Result<List<LookupItemDto<Guid>>>.Successful(tenants);
        }
    }
    #endregion
}
