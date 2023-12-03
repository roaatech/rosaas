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



        #region Client Secrets    

        [HttpGet("{productOwnerClientId}/{productId}")]
        public async Task<IActionResult> GetClientIdByProductAsync([FromRoute] Guid productId, [FromRoute] Guid ProductOwnerClientId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientService.GetClientIdByProductAsync(new GetClientByProductModel(productId, ProductOwnerClientId), cancellationToken));
        }

        [HttpGet("Secrets/{productOwnerClientId}/{productId}")]
        public async Task<IActionResult> GetClientSecretsListAsync([FromRoute] Guid productId, [FromRoute] Guid ProductOwnerClientId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _clientSecretService.GetClientSecretsListAsync(new GetClientByProductModel(productId, ProductOwnerClientId), cancellationToken));
        }


        [HttpPost("{clientRecordId}/Secrets")]
        public async Task<IActionResult> CreateClientSecretAsync([FromBody] CreateClientSecretModel model, [FromRoute] int clientRecordId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientSecretService.CreateClientSecretAsync(model, clientRecordId, cancellationToken));
        }

        [HttpPost("Secrets/{productOwnerClientId}/{productId}")]
        public async Task<IActionResult> CreateClientSecretAsync([FromBody] CreateClientSecretModel model, [FromRoute] Guid productId, [FromRoute] Guid ProductOwnerClientId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientSecretService.CreateClientSecretAsync(model, productId, ProductOwnerClientId, cancellationToken));
        }


        [HttpPut("{clientRecordId}/Secrets/{seretId}")]
        public async Task<IActionResult> UpdateClientSecretAsync([FromBody] UpdateClientSecretModel model, [FromRoute] int clientRecordId, [FromRoute] int seretId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _clientSecretService.UpdateClientSecretAsync(model, clientRecordId, seretId, cancellationToken));
        }


        [HttpPost("{clientRecordId}/Secrets/{seretId}/Regenerate")]
        public async Task<IActionResult> RegenerateClientSecretAsync([FromRoute] int clientRecordId, [FromRoute] int seretId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _clientSecretService.RegenerateClientSecretAsync(clientRecordId, seretId, cancellationToken));
        }


        [HttpDelete("{clientRecordId}/Secrets/{seretId}")]
        public async Task<IActionResult> DeleteClientSecretAsync([FromRoute] int clientRecordId, [FromRoute] int seretId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _clientSecretService.DeleteClientSecretAsync(clientRecordId, seretId, cancellationToken));
        }

        #endregion


        #endregion


    }
}
