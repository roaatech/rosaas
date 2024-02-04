using FluentValidation;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Validators;
using Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Common.Utilities;

namespace Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets
{
    public class ClientSecretService : IClientSecretService
    {
        #region Props 
        private readonly ILogger<ClientSecretService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IIdentityServerConfigurationDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        #endregion


        #region Corts
        public ClientSecretService(
            ILogger<ClientSecretService> logger,
            IIdentityContextService identityContextService,
            IPermissionService permissionService,
            IIdentityServerConfigurationDbContext dbContext)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _permissionService = permissionService;
            _dbContext = dbContext;
        }
        #endregion

        #region Services   

        public async Task<Result<List<ClientSecretDto>>> GetClientSecretsListAsync(GetClientByProductModel model, CancellationToken cancellationToken = default)
        {
            var dtos = await (from s in _dbContext.ClientSecrets
                              join c in _dbContext.Clients on s.ClientId equals c.Id
                              join cc in _dbContext.ClientCustomDetails on c.Id equals cc.ClientId
                              where cc.ProductId == model.ProductId &&
                                    cc.ProductOwnerClientId == model.ProductOwnerClientId
                              select new ClientSecretDto
                              {
                                  Id = s.Id,
                                  ClientRecordId = s.ClientId,
                                  ClientId = c.ClientId,
                                  Created = s.Created,
                                  Description = s.Description,
                                  Expiration = s.Expiration,
                              }).ToListAsync(cancellationToken);

            return Result<List<ClientSecretDto>>.Successful(dtos);
        }



        public async Task<Result<List<ClientSecretDto>>> GetClientSecretsListByClientIdAsync(int clientRecordId, Guid productId, CancellationToken cancellationToken = default)
        {
            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                        _identityContextService.GetUserType(),
                                                        productId,
                                                        EntityType.Product,
                                                        cancellationToken))
            {
                return Result<List<ClientSecretDto>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }
            var dtos = await (from s in _dbContext.ClientSecrets
                              join c in _dbContext.Clients on s.ClientId equals c.Id
                              where s.ClientId == clientRecordId

                              select new ClientSecretDto
                              {
                                  Id = s.Id,
                                  ClientRecordId = s.ClientId,
                                  ClientId = c.ClientId,
                                  Created = s.Created,
                                  Description = s.Description,
                                  Expiration = s.Expiration,
                              }).ToListAsync(cancellationToken);

            return Result<List<ClientSecretDto>>.Successful(dtos);
        }



        public async Task<Result<ClientSecretCreatedResult>> CreateClientSecretAsync(CreateClientSecretModel model, int clientRecordId, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreateClientSecretModelValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<ClientSecretCreatedResult>.New().WithErrors(fValidation.Errors);
            }


            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                        _identityContextService.GetUserType(),
                                                        productId,
                                                        EntityType.Product,
                                                        cancellationToken))
            {
                return Result<ClientSecretCreatedResult>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }
            #endregion

            return await CreateSecretAsync(new CreateClientSecretModel(model.Description, model.Expiration), clientRecordId, cancellationToken);
        }



        public async Task<Result<ClientSecretCreatedResult>> CreateClientSecretAsync(CreateClientSecretModel model, Guid productId, Guid ProductOwnerClientId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreateClientSecretModelValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<ClientSecretCreatedResult>.New().WithErrors(fValidation.Errors);
            }
            #endregion

            var client = await _dbContext.Clients
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
                               .Where(x => x.ProductId == productId &&
                                           x.ProductOwnerClientId == ProductOwnerClientId)
                               .Select(x => new
                               {
                                   ClientRecordId = x.Id,
                                   ClientId = x.ClientId,
                               })
                               .FirstOrDefaultAsync(cancellationToken);

            if (client is null)
            {
                return Result<ClientSecretCreatedResult>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            return await CreateSecretAsync(new CreateClientSecretModel(model.Description, model.Expiration), client.ClientRecordId, cancellationToken);
        }

        public async Task<Result<ClientSecretCreatedResult>> CreateSecretAsync(CreateClientSecretModel model, int clientRecordId, CancellationToken cancellationToken = default)
        {
            string decryptedSecret = StringGenarator.Random(60);

            var secret = new ClientSecret
            {
                Description = model.Description,
                Expiration = model.Expiration,
                Value = decryptedSecret.Sha256(),
                ClientId = clientRecordId,

            };

            _dbContext.ClientSecrets.Add(secret);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<ClientSecretCreatedResult>.Successful(new ClientSecretCreatedResult(secret.Id, decryptedSecret));
        }


        public async Task<Result> UpdateClientSecretAsync(UpdateClientSecretModel model, int clientRecordId, Guid productId, int seretId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdateClientSecretModelValidator(_identityContextService).Validate(model);
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
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            #endregion

            var clientSecret = await _dbContext.ClientSecrets
                                               .Where(x => x.Id == seretId &&
                                                           x.ClientId == clientRecordId)
                                               .SingleOrDefaultAsync();
            if (clientSecret is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            clientSecret.Description = model.Description;
            clientSecret.Expiration = model.Expiration;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result<string>> RegenerateClientSecretAsync(int clientRecordId, Guid productId, int seretId, CancellationToken cancellationToken = default)
        {
            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                          _identityContextService.GetUserType(),
                                                          productId,
                                                          EntityType.Product,
                                                          cancellationToken))
            {
                return Result<string>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            var clientSecret = await _dbContext.ClientSecrets
                                                .Where(x => x.Id == seretId &&
                                                            x.ClientId == clientRecordId)
                                                .SingleOrDefaultAsync();
            if (clientSecret is null)
            {
                return Result<string>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            string decryptedSecret = StringGenarator.Random(60);

            clientSecret.Value = decryptedSecret.Sha256();

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Successful(decryptedSecret);
        }



        public async Task<Result> DeleteClientSecretAsync(int clientRecordId, Guid productId, int seretId, CancellationToken cancellationToken = default)
        {
            if (!await _permissionService.HasPermissionAsync(_identityContextService.UserId,
                                                            _identityContextService.GetUserType(),
                                                            productId,
                                                            EntityType.Product,
                                                            cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            var clientSecret = await _dbContext.ClientSecrets
                                                .Where(x => x.Id == seretId &&
                                                            x.ClientId == clientRecordId)
                                                .SingleOrDefaultAsync();
            if (clientSecret is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            _dbContext.ClientSecrets.Remove(clientSecret);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        #endregion





    }


}
