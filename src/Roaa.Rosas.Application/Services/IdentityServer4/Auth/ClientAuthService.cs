
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.IdentityServer4.Auth.Models;
using Roaa.Rosas.Application.Services.IdentityServer4.Auth.Validators;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Auth
{
    public class ClientAuthService : IClientAuthService
    {
        #region Props
        private readonly IClientStore _clientStore;
        private readonly ILogger<ClientAuthService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IAuthTokenService _tokenService;
        #endregion


        #region Corts
        public ClientAuthService(
            IClientStore clientStore,
            ILogger<ClientAuthService> logger,
            IIdentityContextService identityContextService,
            IAuthTokenService tokenService)
        {
            _clientStore = clientStore;
            _logger = logger;
            _identityContextService = identityContextService;
            _tokenService = tokenService;
        }
        #endregion

        #region Services   

        public async Task<Result<AuthResultModel>> AuthClientAsync(AuthClientModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new AuthClientValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<AuthResultModel>.New().WithErrors(fValidation.Errors);
            }

            var client = await _clientStore.FindClientByIdAsync(model.ClientId);

            if (client is null)
            {
                return Result<AuthResultModel>.Fail(SystemMessages.ErrorMessage.InvalidClientCredential, _identityContextService.Locale);
            }

            if (!client.ClientSecrets.Any(x => x.Value.Equals(model.ClientSecret.Sha256())))
            {
                return Result<AuthResultModel>.Fail(SystemMessages.ErrorMessage.InvalidClientCredential, _identityContextService.Locale);
            }

            if (!client.Enabled)
            {
                return Result<AuthResultModel>.Fail(SystemMessages.ErrorMessage.AccountDeactivated, _identityContextService.Locale);
            }
            #endregion

            var tokenResult = await _tokenService.GenerateAsync(client);

            if (!tokenResult.Success)
            {
                return Result<AuthResultModel>.Fail(tokenResult.Messages);
            }

            return Result<AuthResultModel>.Successful(new AuthResultModel
            {
                Token = new TokenModel
                {
                    AccessToken = tokenResult.Data.AccessToken,
                },
                Info = new ClientInfoDto()
                //{
                //    RosasClient = new InfoDto
                //    {
                //        Name = "roaa",
                //        Title = "Roaa Tech",
                //    },
                //    RosasProduct = new InfoDto
                //    {
                //        Name = "osos",
                //        Title = "OSOS System",
                //    }
                //}
            });
        }

        #endregion





    }


}
