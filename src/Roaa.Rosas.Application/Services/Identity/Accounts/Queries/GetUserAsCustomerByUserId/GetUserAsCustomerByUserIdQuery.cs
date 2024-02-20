using MediatR;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Identity.Accounts.Queries.GetUserAsCustomerByUserId
{
    public record GetUserAsCustomerByUserIdQuery : IRequest<Result<UserCustomerDto>>
    {
        public GetUserAsCustomerByUserIdQuery(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; init; }
    }
}
