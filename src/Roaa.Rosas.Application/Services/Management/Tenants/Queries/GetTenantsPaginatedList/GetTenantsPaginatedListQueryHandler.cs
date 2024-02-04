using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsPaginatedList
{
    public class GetTenantsPaginatedListQueryHandler : IRequestHandler<GetTenantsPaginatedListQuery, PaginatedResult<TenantListItemDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public GetTenantsPaginatedListQueryHandler(IRosasDbContext dbContext,
                                                  IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }
        #endregion



        #region Handler   
        public async Task<PaginatedResult<TenantListItemDto>> Handle(GetTenantsPaginatedListQuery request, CancellationToken cancellationToken)
        {

            var query = _dbContext.Tenants
                                  .AsNoTracking()
                                   .Where(x => _identityContextService.IsSuperAdmin() ||
                                                _dbContext.EntityAdminPrivileges
                                                                    .Any(a =>
                                                                            a.UserId == _identityContextService.UserId &&
                                                                            a.EntityId == x.Id &&
                                                                            a.EntityType == EntityType.Tenant
                                                                            )
                                          )
                                    .Include(x => x.Subscriptions)
                                    .ThenInclude(x => x.Product)
                                    .Select(tenant => new TenantListItemDto
                                    {
                                        Id = tenant.Id,
                                        SystemName = tenant.SystemName,
                                        DisplayName = tenant.DisplayName,
                                        Subscriptions = tenant.Subscriptions.Select(x => new SubscriptionDto
                                        {
                                            ProductId = x.ProductId,
                                            ProductName = x.Product.DisplayName,
                                            SubscriptionId = x.Id,
                                        }),
                                        CreatedDate = tenant.CreationDate,
                                        EditedDate = tenant.ModificationDate,
                                    });

            var sort = request.Sort.HandleDefaultSorting(new string[] { "SystemName", "DisplayName", "ProductId", "Status", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(request.Filters, new string[] { "_SystemName", "_DisplayName", "ProductId", "Status" }, "CreatedDate");

            query = query.OrderBy(sort);

            var pagedUsers = await query.ToPagedResultAsync(request.PaginationInfo, cancellationToken);

            return pagedUsers;
        }
        #endregion
    }
}
