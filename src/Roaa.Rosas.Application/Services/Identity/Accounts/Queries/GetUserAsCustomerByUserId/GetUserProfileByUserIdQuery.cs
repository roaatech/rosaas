using MediatR;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Identity.Accounts.Queries.GetUserProfileByUserId
{
    public record GetUserProfileByUserIdQuery : IRequest<Result<UserProfileDto>>
    {
        public GetUserProfileByUserIdQuery(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; init; }
    }
}
