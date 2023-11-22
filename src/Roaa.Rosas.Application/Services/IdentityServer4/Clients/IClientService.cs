using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients
{
    public interface IClientService
    {
        Task<Result> CreateClientAsync(CreateClientByProductModel model, CancellationToken cancellationToken = default);

        Task<Result<GetClientByProductDto>> GetClientIdByProductAsync(GetClientByProductModel model, CancellationToken cancellationToken = default);
    }
}
