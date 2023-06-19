using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantById
{
    public record GetTenantByIdQuery : IRequest<Result<TenantDto>>
    {
        public GetTenantByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; init; }
    }
}
