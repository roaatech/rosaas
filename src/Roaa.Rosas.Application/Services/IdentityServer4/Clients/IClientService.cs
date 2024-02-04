using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients
{
    public interface IClientService
    {
        Task<Result<List<ClientDto>>> GetClientsListByProductAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result> CreateClientAsExternalSystemAsync(CreateClientAsExternalSystemModel model, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<int>>> CreateClientAsExternalSystemClientAsync(CreateClientAsExternalSystemClientModel model, Guid productOwnerClientId, Guid productId, CancellationToken cancellationToken = default);

        Task<Result<ClientDto>> GetClientIdOfExternalSystemAsync(GetClientOfExternalSystemModel model, CancellationToken cancellationToken = default);

        Task<Result> UpdateClientByProductAsync(UpdateClientByProductModel model, int clientRecordId, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> DeleteClientByProductAsync(int clientRecordId, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> ActivateClientByProductAsync(ActivateClientModel model, int clientRecordId, Guid productId, CancellationToken cancellationToken = default);

    }
}
