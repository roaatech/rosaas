using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantStatusById
{
    public record GetTenantStatusByIdQuery : IRequest<Result<TenantStatusDto>>
    {
        public GetTenantStatusByIdQuery(Guid id, Guid productId)
        {
            Id = id;
            ProductId = productId;
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
    }
}
