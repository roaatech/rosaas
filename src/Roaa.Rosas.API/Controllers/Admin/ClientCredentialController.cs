using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets;
using Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    [Authorize(Policy = AuthPolicy.Identity.ClientCredential, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ClientCredentialController : BaseIdentityApiController
    {
        #region Props 
        private readonly ILogger<ClientCredentialController> _logger;
        private readonly IClientSecretService _clientSecretService;
        private readonly IClientService _clientService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public ClientCredentialController(ILogger<ClientCredentialController> logger,
                                IWebHostEnvironment environment,
                                IClientSecretService clientSecretService,
                                IClientService clientService
                                )
        {
            _logger = logger;
            _environment = environment;
            _clientSecretService = clientSecretService;
            _clientService = clientService;
        }
        #endregion


        #region Actions   

        [HttpGet("ExternalSystem/{productOwnerClientId}/{productId}")]
        public async Task<IActionResult> GetClientIdOfExternalSystemAsync([FromRoute] Guid productId, [FromRoute] Guid ProductOwnerClientId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientService.GetClientIdOfExternalSystemAsync(new GetClientOfExternalSystemModel(productId, ProductOwnerClientId, Domain.Entities.ClientType.ExternalSystem), cancellationToken));
        }


        #region Clients   


        // New
        [HttpGet("{productOwnerClientId}/{productId}")]
        public async Task<IActionResult> GetClientsListByProductAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _clientService.GetClientsListByProductAsync(productId, cancellationToken));
        }

        [HttpPost("{productOwnerClientId}/{productId}")]
        public async Task<IActionResult> CreateClientAsExternalSystemClientAsync([FromBody] CreateClientAsExternalSystemClientModel model,
                                                                                 [FromRoute] Guid productOwnerClientId,
                                                                                 [FromRoute] Guid productId,
                                                                                 CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientService.CreateClientAsExternalSystemClientAsync(model, productOwnerClientId, productId, cancellationToken));
        }

        [HttpPut("{clientRecordId}/{productId}")]
        public async Task<IActionResult> UpdateClientByProductAsync([FromBody] UpdateClientByProductModel model, [FromRoute] int clientRecordId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _clientService.UpdateClientByProductAsync(model, clientRecordId, productId, cancellationToken));
        }

        [HttpPost("{clientRecordId}/{productId}/active")]
        public async Task<IActionResult> ActivateClientByProductAsync([FromBody] ActivateClientModel model, [FromRoute] int clientRecordId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientService.ActivateClientByProductAsync(model, clientRecordId, productId, cancellationToken));
        }

        [HttpDelete("{clientRecordId}/{productId}")]
        public async Task<IActionResult> DeleteClientByProductAsync([FromRoute] int clientRecordId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _clientService.DeleteClientByProductAsync(clientRecordId, productId, cancellationToken));
        }

        #endregion



        #region Client Secrets    
        // New
        [HttpGet("{clientRecordId}/{productId}/Secrets")]
        public async Task<IActionResult> GetClientSecretsListByClientIdAsync([FromRoute] int clientRecordId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _clientSecretService.GetClientSecretsListByClientIdAsync(clientRecordId, productId, cancellationToken));
        }

        [HttpPost("{clientRecordId}/{productId}/Secrets")]
        public async Task<IActionResult> CreateClientSecretAsync([FromBody] CreateClientSecretModel model, [FromRoute] int clientRecordId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientSecretService.CreateClientSecretAsync(model, clientRecordId, productId, cancellationToken));
        }

        [HttpPut("{clientRecordId}/{productId}/Secrets/{seretId}")]
        public async Task<IActionResult> UpdateClientSecretAsync([FromBody] UpdateClientSecretModel model, [FromRoute] int clientRecordId, [FromRoute] Guid productId, [FromRoute] int seretId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _clientSecretService.UpdateClientSecretAsync(model, clientRecordId, productId, seretId, cancellationToken));
        }


        [HttpPost("{clientRecordId}/{productId}/Secrets/{seretId}/Regenerate")]
        public async Task<IActionResult> RegenerateClientSecretAsync([FromRoute] int clientRecordId, [FromRoute] Guid productId, [FromRoute] int seretId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientSecretService.RegenerateClientSecretAsync(clientRecordId, productId, seretId, cancellationToken));
        }


        [HttpDelete("{clientRecordId}/{productId}/Secrets/{seretId}")]
        public async Task<IActionResult> DeleteClientSecretAsync([FromRoute] int clientRecordId, [FromRoute] Guid productId, [FromRoute] int seretId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _clientSecretService.DeleteClientSecretAsync(clientRecordId, productId, seretId, cancellationToken));
        }

        #endregion


        #endregion


    }
}
