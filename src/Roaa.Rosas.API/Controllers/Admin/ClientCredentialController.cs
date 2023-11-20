using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets;
using Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    public class ClientCredentialController : BaseSuperAdminIdentityApiController
    {
        #region Props 
        private readonly ILogger<ClientCredentialController> _logger;
        private readonly IClientSecretService _clientSecretService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public ClientCredentialController(ILogger<ClientCredentialController> logger,
                                IWebHostEnvironment environment,
                                IClientSecretService clientSecretService)
        {
            _logger = logger;
            _environment = environment;
            _clientSecretService = clientSecretService;
        }
        #endregion


        #region Actions   



        #region Client Secrets    

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
