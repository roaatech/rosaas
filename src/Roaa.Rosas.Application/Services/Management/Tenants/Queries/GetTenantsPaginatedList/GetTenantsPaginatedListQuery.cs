using MediatR;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsPaginatedList
{
    public record GetTenantsPaginatedListQuery : IRequest<PaginatedResult<TenantListItemDto>>
    {
        public GetTenantsPaginatedListQuery(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem? sort)
        {
            PaginationInfo = paginationInfo;
            Filters = filters;
            Sort = sort;
        }

        public PaginationMetaData PaginationInfo { get; init; } = new();
        public List<FilterItem> Filters { get; init; } = new();
        public SortItem? Sort { get; init; }
    }
}
