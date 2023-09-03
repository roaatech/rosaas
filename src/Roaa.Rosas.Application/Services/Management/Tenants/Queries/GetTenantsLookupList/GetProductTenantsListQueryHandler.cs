using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsLookupList
{
    public class GetTenantsLookupListAsyncQueryHandler : IRequestHandler<GetTenantsLookupListQuery, Result<List<LookupItemDto<Guid>>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public GetTenantsLookupListAsyncQueryHandler(IRosasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion


        #region Handler   
        public async Task<Result<List<LookupItemDto<Guid>>>> Handle(GetTenantsLookupListQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.Tenants
                                              .AsNoTracking()
                                              .Select(x => new LookupItemDto<Guid>
                                              {
                                                  Id = x.Id,
                                                  Name = x.UniqueName,
                                              })
                                              .ToListAsync(cancellationToken);

            return Result<List<LookupItemDto<Guid>>>.Successful(tenants);
        }
    }
    #endregion
}
