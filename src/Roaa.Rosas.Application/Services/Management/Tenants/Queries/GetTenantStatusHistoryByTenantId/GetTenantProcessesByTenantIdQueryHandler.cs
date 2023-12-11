using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantStatusHistoryByTenantId
{
    public class GetTenantProcessesByTenantIdQueryHandler : IRequestHandler<GetTenantProcessesByTenantIdQuery, Result<List<TenantProcessDto>>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetTenantProcessesByTenantIdQueryHandler(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<List<TenantProcessDto>>> Handle(GetTenantProcessesByTenantIdQuery request, CancellationToken cancellationToken)
        {
            var results = await _dbContext.TenantStatusHistory
                                                .AsNoTracking()
                                                .Where(x => _identityContextService.IsSuperAdmin() ||
                                                            _dbContext.EntityAdminPrivileges
                                                                        .Any(a =>
                                                                            a.UserId == _identityContextService.UserId &&
                                                                            a.EntityId == x.Id &&
                                                                            a.EntityType == EntityType.Tenant
                                                                            )
                                                        )
                                                  .Where(x => x.TenantId == request.TenantId && x.ProductId == request.ProductId)
                                                  .Select(x => new TenantProcessDto
                                                  {
                                                      TenantId = x.TenantId,
                                                      Status = x.Status,
                                                      Step = x.Step,
                                                      PreviousStatus = x.PreviousStatus,
                                                      PreviousStep = x.PreviousStep,
                                                      OwnerId = x.OwnerId,
                                                      OwnerType = x.OwnerType,
                                                      Message = x.Message,
                                                      Created = x.CreationDate,
                                                  })
                                                  .OrderByDescending(x => x.Created)
                                                  .ToListAsync(cancellationToken);

            return Result<List<TenantProcessDto>>.Successful(results);
        }
        #endregion
    }
}
