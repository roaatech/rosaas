using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.IdentityServer4.Auth.Models;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Validators;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients
{
    public class ClientService : IClientService
    {
        #region Props
        private readonly IClientStore _clientStore;
        private readonly ILogger<ClientService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IAuthTokenService _tokenService;
        private readonly IIdentityServerConfigurationDbContext _dbContext;
        private readonly IPublisher _publisher;
        #endregion


        #region Corts
        public ClientService(
            IClientStore clientStore,
            ILogger<ClientService> logger,
            IIdentityContextService identityContextService,
            IIdentityServerConfigurationDbContext dbContext,
            IAuthTokenService tokenService,
            IPublisher publisher)
        {
            _clientStore = clientStore;
            _logger = logger;
            _identityContextService = identityContextService;
            _dbContext = dbContext;
            _tokenService = tokenService;
            _publisher = publisher;
        }
        #endregion

        #region Services   





        public async Task<Result> CreateClientAsync(CreateClientByProductModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreateClientByProductModelValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }


            var clientId = model.ClientId;

            if (await _dbContext.Clients.AnyAsync(c => c.ClientId == model.ClientId, cancellationToken))
            {
                clientId = $"{model.ClientId}-{Guid.NewGuid()}";

                if (_dbContext.Clients.Any(c => c.ClientId == clientId))
                {
                    return Result<AuthResultModel>.Fail(CommonErrorKeys.ResourceAlreadyExists, _identityContextService.Locale);
                }
            }
            #endregion

            var client = new Client()
            {
                ClientId = clientId.ToLower(),
                ClientName = model.DisplayName,
                Description = model.Description,

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                RequireClientSecret = true,
                AllowAccessTokensViaBrowser = false,
                AllowedScopes = { SystemConsts.Scopes.ExternalSystem, },
                AccessTokenLifetime = 3600, //in seconds = 1hour
                AccessTokenType = AccessTokenType.Jwt,

                // secret for authentication
                ClientSecrets =
                    {
                        new Secret(model.ProductId.ToString().Sha256())
                    },
                Properties = new Dictionary<string, string>
                    {
                        {SystemConsts.Clients.Properties.RosasClientId , model.ProductOwnerClientId.ToString()},
                        {SystemConsts.Clients.Properties.RosasProductId ,model.ProductId.ToString()},
                        {SystemConsts.Clients.Properties.RosasUserId ,model.ProductId.ToString()},
                    },
                Claims = new List<ClientClaim>
                    {
                        new ClientClaim(SystemConsts.Clients.Claims.ClaimType,SystemConsts.Clients.Claims.ExternalSystem)
                    }
            };

            var entity = client.ToEntity();

            var clientCustomDetail = new Domain.Entities.ClientCustomDetail
            {
                ClientId = entity.Id,
                ProductId = model.ProductId,
                UserId = model.ProductId,
                ProductOwnerClientId = model.ProductOwnerClientId,
            };

            _dbContext.Clients.Add(entity);

            _dbContext.ClientCustomDetails.Add(clientCustomDetail);

            if (await _dbContext.SaveChangesAsync(cancellationToken) > 0)
            {
                await _publisher.Publish(new IdentityServerClientCreatedEvent(entity, clientCustomDetail));
            }

            return Result.Successful();
        }


        public async Task<Result<GetClientByProductDto>> GetClientIdByProductAsync(GetClientByProductModel model, CancellationToken cancellationToken = default)
        {

            var dto = await _dbContext.Clients
                                 .Join(
                                      _dbContext.ClientCustomDetails,
                                      c => c.Id,
                                      cc => cc.ClientId,
                                      (c, cc) => new
                                      {
                                          c.Id,
                                          c.ClientId,
                                          cc.ProductId,
                                          cc.ProductOwnerClientId,
                                      })
                                 .Where(x => x.ProductId == model.ProductId &&
                                             x.ProductOwnerClientId == model.ProductOwnerClientId)
                                 .Select(x => new GetClientByProductDto()
                                 {
                                     ClientRecordId = x.Id,
                                     ClientId = x.ClientId,
                                 })
                                 .FirstOrDefaultAsync(cancellationToken);

            return Result<GetClientByProductDto>.Successful(dto);
        }

        #endregion





    }


}
