using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Validators;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
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
        private readonly IPermissionService _permissionService;
        private readonly IIdentityServerConfigurationDbContext _IdentityServerDbContext;
        private readonly IRosasDbContext _rosasDbContext;
        private readonly IPublisher _publisher;
        #endregion


        #region Corts
        public ClientService(
            IClientStore clientStore,
            ILogger<ClientService> logger,
            IIdentityContextService identityContextService,
            IPermissionService permissionService,
            IIdentityServerConfigurationDbContext dbContext,
            IRosasDbContext rosasDbContext,
            IAuthTokenService tokenService,
            IPublisher publisher)
        {
            _clientStore = clientStore;
            _logger = logger;
            _identityContextService = identityContextService;
            _permissionService = permissionService;
            _IdentityServerDbContext = dbContext;
            rosasDbContext = _rosasDbContext;
            _tokenService = tokenService;
            _publisher = publisher;
        }
        #endregion

        #region Services   






        public async Task<Result> CreateClientAsExternalSystemAsync(CreateClientAsExternalSystemModel model, CancellationToken cancellationToken = default)
        {
            return await CreateClientAsync(
                                            new CreateClientModel
                                            {
                                                ClientId = model.ClientId,
                                                Description = model.Description,
                                                DisplayName = model.DisplayName,
                                                ProductId = model.ProductId,
                                                ProductOwnerClientId = model.ProductOwnerClientId,
                                                AccessTokenLifetimeInHour = model.AccessTokenLifetimeInHour,
                                                ClientType = Domain.Entities.ClientType.ExternalSystem,
                                                AllowGenerateClientId = true,
                                            });
        }

        public async Task<Result<CreatedResult<int>>> CreateClientAsExternalSystemClientAsync(CreateClientAsExternalSystemClientModel model, Guid productOwnerClientId, Guid productId, CancellationToken cancellationToken = default)
        {
            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                           _identityContextService.GetUserType(),
                                                           productId,
                                                           EntityType.Product,
                                                           cancellationToken))
            {
                return Result<CreatedResult<int>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            return await CreateClientAsync(
                                            new CreateClientModel
                                            {
                                                ClientId = model.ClientId,
                                                Description = model.Description,
                                                DisplayName = model.DisplayName,
                                                ProductId = productId,
                                                ProductOwnerClientId = productOwnerClientId,
                                                AccessTokenLifetimeInHour = model.AccessTokenLifetimeInHour,
                                                ClientType = Domain.Entities.ClientType.ExternalSystemClient
                                            });
        }



        private async Task<Result<CreatedResult<int>>> CreateClientAsync(CreateClientModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreateClientByProductModelValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<int>>.New().WithErrors(fValidation.Errors);
            }


            var clientId = model.ClientId;

            if (await _IdentityServerDbContext.Clients.AnyAsync(c => c.ClientId == model.ClientId, cancellationToken))
            {
                if (model.AllowGenerateClientId)
                {
                    clientId = $"{model.ClientId}-{Guid.NewGuid()}";

                    if (await _IdentityServerDbContext.Clients.AnyAsync(c => c.ClientId == clientId, cancellationToken))
                    {
                        return Result<CreatedResult<int>>.Fail(CommonErrorKeys.ResourceAlreadyExists, _identityContextService.Locale);
                    }
                }
                else
                {
                    return Result<CreatedResult<int>>.Fail(CommonErrorKeys.ResourceAlreadyExists, _identityContextService.Locale);
                }
            }
            #endregion

            var newId = Guid.NewGuid();

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
                AccessTokenLifetime = model.AccessTokenLifetimeInHour * 60 * 60, //in seconds = 1hour
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
                        {SystemConsts.Clients.Properties.RosasUserId ,newId.ToString()},
                },
                Claims = new List<ClientClaim>
                {
                    new ClientClaim(SystemConsts.Clients.Claims.ClaimType,  ClientTypeManager.FromKey(model.ClientType).GetClaimTypeValue() )
                }
            };
            var entity = client.ToEntity();

            var clientCustomDetail = new Domain.Entities.ClientCustomDetail
            {
                ClientId = entity.Id,
                ProductId = model.ProductId,
                UserId = newId,
                ProductOwnerClientId = model.ProductOwnerClientId,
                ClientType = model.ClientType,
            };

            _IdentityServerDbContext.Clients.Add(entity);

            _IdentityServerDbContext.ClientCustomDetails.Add(clientCustomDetail);

            if (await _IdentityServerDbContext.SaveChangesAsync(cancellationToken) > 0)
            {
                await _publisher.Publish(new IdentityServerClientCreatedEvent(entity, clientCustomDetail));
            }

            return Result<CreatedResult<int>>.Successful(new CreatedResult<int>(entity.Id));
        }


        public async Task<Result<ClientDto>> GetClientIdOfExternalSystemAsync(GetClientOfExternalSystemModel model, CancellationToken cancellationToken = default)
        {

            var dto = await _IdentityServerDbContext.Clients
                                 .Join(
                                      _IdentityServerDbContext.ClientCustomDetails,
                                      c => c.Id,
                                      cc => cc.ClientId,
                                      (c, cc) => new
                                      {
                                          c.Id,
                                          c.ClientId,
                                          c.Description,
                                          c.ClientName,
                                          c.Enabled,
                                          c.AccessTokenLifetime,
                                          c.Created,
                                          cc.ProductId,
                                          cc.ProductOwnerClientId,
                                          cc.ClientType,
                                      })
                                 .Where(x => x.ProductId == model.ProductId &&
                                             x.ProductOwnerClientId == model.ProductOwnerClientId &&
                                             x.ClientType == model.ClientType)
                                 .Select(x => new ClientDto()
                                 {
                                     Id = x.Id,
                                     ClientRecordId = x.Id,
                                     ClientId = x.ClientId,
                                     ClientType = x.ClientType,
                                     Description = x.Description,
                                     DisplayName = x.ClientName,
                                     CreatedDate = x.Created,
                                     IsActive = x.Enabled,
                                     AccessTokenLifetimeInHour = (x.AccessTokenLifetime / 3600),
                                 })
                                 .FirstOrDefaultAsync(cancellationToken);

            return Result<ClientDto>.Successful(dto);
        }



        public async Task<Result<List<ClientDto>>> GetClientsListByProductAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                            _identityContextService.GetUserType(),
                                                            productId,
                                                            EntityType.Product,
                                                            cancellationToken))
            {
                return Result<List<ClientDto>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            var dto = await _IdentityServerDbContext.Clients
                                 .Join(
                                      _IdentityServerDbContext.ClientCustomDetails,
                                      c => c.Id,
                                      cc => cc.ClientId,
                                      (c, cc) => new
                                      {
                                          c.Id,
                                          c.ClientId,
                                          c.Description,
                                          c.ClientName,
                                          c.Enabled,
                                          c.AccessTokenLifetime,
                                          c.Created,
                                          cc.ProductId,
                                          cc.ProductOwnerClientId,
                                          cc.ClientType,
                                      })
                                 .Where(x => x.ProductId == productId)
                                 .Select(x => new ClientDto()
                                 {
                                     Id = x.Id,
                                     ClientRecordId = x.Id,
                                     ClientId = x.ClientId,
                                     ClientType = x.ClientType,
                                     Description = x.Description,
                                     DisplayName = x.ClientName,
                                     CreatedDate = x.Created,
                                     IsActive = x.Enabled,
                                     AccessTokenLifetimeInHour = (x.AccessTokenLifetime / 3600),
                                 })
                                 .ToListAsync(cancellationToken);

            return Result<List<ClientDto>>.Successful(dto);
        }





        public async Task<Result> UpdateClientByProductAsync(UpdateClientByProductModel model, int clientRecordId, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdateClientByProductModelValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                                  _identityContextService.GetUserType(),
                                                                  productId,
                                                                  EntityType.Product,
                                                                  cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }

            if (!await _IdentityServerDbContext.ClientCustomDetails
                                               .Where(x => x.ClientId == clientRecordId &&
                                                           x.ProductId == productId)
                                               .AnyAsync(cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion

            var client = await _IdentityServerDbContext.Clients
                                   .Where(x => x.Id == clientRecordId)
                                   .FirstOrDefaultAsync(cancellationToken);
            if (client is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            client.Description = model.Description;
            client.ClientName = model.DisplayName;
            client.AccessTokenLifetime = model.AccessTokenLifetimeInHour * 60 * 60;

            await _IdentityServerDbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public async Task<Result> DeleteClientByProductAsync(int clientRecordId, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation

            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                                  _identityContextService.GetUserType(),
                                                                  productId,
                                                                  EntityType.Product,
                                                                  cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }

            var clientCustomDetails = await _IdentityServerDbContext.ClientCustomDetails
                                                                       .Where(x => x.ClientId == clientRecordId &&
                                                                                   x.ProductId == productId &&
                                                                                   x.ClientType == Domain.Entities.ClientType.ExternalSystemClient)
                                                                       .SingleOrDefaultAsync(cancellationToken);
            if (clientCustomDetails is null)
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion

            var client = await _IdentityServerDbContext.Clients
                                   .Where(x => x.Id == clientRecordId)
                                   .FirstOrDefaultAsync(cancellationToken);
            if (client is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            _IdentityServerDbContext.ClientSecrets.RemoveRange(await _IdentityServerDbContext.ClientSecrets
                                                                                             .Where(x => x.ClientId == clientRecordId)
                                                                                             .ToListAsync(cancellationToken));

            _IdentityServerDbContext.ClientProperties.RemoveRange(await _IdentityServerDbContext.ClientProperties
                                                                                                .Where(x => x.ClientId == clientRecordId)
                                                                                                .ToListAsync(cancellationToken));

            _IdentityServerDbContext.ClientClaims.RemoveRange(await _IdentityServerDbContext.ClientClaims
                                                                                            .Where(x => x.ClientId == clientRecordId)
                                                                                            .ToListAsync(cancellationToken));

            _IdentityServerDbContext.ClientCustomDetails.Remove(clientCustomDetails);


            _IdentityServerDbContext.Clients.Remove(client);


            await _IdentityServerDbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public async Task<Result> ActivateClientByProductAsync(ActivateClientModel model, int clientRecordId, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation

            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                                  _identityContextService.GetUserType(),
                                                                  productId,
                                                                  EntityType.Product,
                                                                  cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }

            if (!await _IdentityServerDbContext.ClientCustomDetails
                                                .Where(x => x.ClientId == clientRecordId &&
                                                            x.ProductId == productId &&
                                                            x.ClientType == Domain.Entities.ClientType.ExternalSystemClient)
                                                .AnyAsync(cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion

            var client = await _IdentityServerDbContext.Clients
                                   .Where(x => x.Id == clientRecordId)
                                   .FirstOrDefaultAsync(cancellationToken);
            if (client is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            client.Enabled = model.IsActive;

            await _IdentityServerDbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        #endregion





    }


}
