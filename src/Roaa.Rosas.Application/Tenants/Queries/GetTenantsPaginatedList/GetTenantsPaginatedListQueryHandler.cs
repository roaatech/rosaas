using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantsPaginatedList
{
    public class GetTenantsPaginatedListQueryHandler : IRequestHandler<GetTenantsPaginatedListQuery, PaginatedResult<TenantListItemDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion

        #region Corts
        public GetTenantsPaginatedListQueryHandler(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<PaginatedResult<TenantListItemDto>> Handle(GetTenantsPaginatedListQuery request, CancellationToken cancellationToken)
        {

            var query = _dbContext.Tenants.AsNoTracking()
                                                     .Include(x => x.Products)
                                                     .ThenInclude(x => x.Product)
                                                     .Select(tenant => new TenantListItemDto
                                                     {
                                                         Id = tenant.Id,
                                                         UniqueName = tenant.UniqueName,
                                                         Title = tenant.Title,
                                                         Products = tenant.Products.Select(x => new
                                                                            LookupItemDto<Guid>(x.ProductId, x.Product.Name)),
                                                         //Status = tenant.Status,
                                                         CreatedDate = tenant.Created,
                                                         EditedDate = tenant.Edited,
                                                     });

            var sort = request.Sort.HandleDefaultSorting(new string[] { "TenantUniqueName", "TenantTitle", "ProductId", "Status", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(request.Filters, new string[] { "_UniqueName", "_Title", "ProductId", "Status" }, "CreatedDate");

            query = query.OrderBy(sort);

            var pagedUsers = await query.ToPagedResultAsync(request.PaginationInfo, cancellationToken);

            return pagedUsers;
        }
        #endregion
    }
}
