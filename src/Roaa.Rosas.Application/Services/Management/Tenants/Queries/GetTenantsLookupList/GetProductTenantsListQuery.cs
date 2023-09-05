using MediatR;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsLookupList
{
    public record GetTenantsLookupListQuery : IRequest<Result<List<LookupItemDto<Guid>>>>
    {
        public GetTenantsLookupListQuery()
        {
        }

    }
}
