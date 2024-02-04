using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets
{
    public interface IClientSecretService
    {
        Task<Result<List<ClientSecretDto>>> GetClientSecretsListByClientIdAsync(int clientRecordId, Guid productId, CancellationToken cancellationToken = default);

        Task<Result<List<ClientSecretDto>>> GetClientSecretsListAsync(GetClientByProductModel model, CancellationToken cancellationToken = default);

        Task<Result<ClientSecretCreatedResult>> CreateClientSecretAsync(CreateClientSecretModel model, int clientRecordId, Guid productId, CancellationToken cancellationToken = default);

        Task<Result<ClientSecretCreatedResult>> CreateClientSecretAsync(CreateClientSecretModel model, Guid productId, Guid ProductOwnerClientId, CancellationToken cancellationToken = default);

        Task<Result> UpdateClientSecretAsync(UpdateClientSecretModel model, int clientRecordId, Guid productId, int seretId, CancellationToken cancellationToken = default);

        Task<Result<string>> RegenerateClientSecretAsync(int clientRecordId, Guid productId, int seretId, CancellationToken cancellationToken = default);

        Task<Result> DeleteClientSecretAsync(int clientRecordId, Guid productId, int seretId, CancellationToken cancellationToken = default);
    }
}
