using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Application.Services.Management.Products.Validators;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Products
{
    public partial class ProductService : IProductService
    {
        #region Props 
        private readonly ILogger<ProductService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public ProductService(
            ILogger<ProductService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services   


        public async Task<Result<List<ProductUrlListItem>>> GetProductsUrlsByTenantIdAsync(Guid tenantId, Expression<Func<Subscription, ProductUrlListItem>> selector, CancellationToken cancellationToken = default)
        {
            var urls = await _dbContext.Subscriptions.AsNoTracking()
                                                  .Include(x => x.Product)
                                                   .Where(x => x.TenantId == tenantId)
                                                   .Select(selector)
                                                   .ToListAsync(cancellationToken);

            return Result<List<ProductUrlListItem>>.Successful(urls);
        }

        public async Task<Result<T>> GetProductEndpointByIdAsync<T>(Guid productId, Expression<Func<Product, T>> selector, CancellationToken cancellationToken = default)
        {
            var url = await _dbContext.Products.AsNoTracking()
                                                   .Where(x => x.Id == productId)
                                                   .Select(selector)
                                                   .SingleOrDefaultAsync(cancellationToken);

            return Result<T>.Successful(url);
        }

        public async Task<PaginatedResult<ProductListItemDto>> GetProductsPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Products.AsNoTracking()
                                          .Include(x => x.Client)
                                          .Select(product => new ProductListItemDto
                                          {
                                              Id = product.Id,
                                              DefaultHealthCheckUrl = product.DefaultHealthCheckUrl,
                                              Name = !string.IsNullOrWhiteSpace(product.SystemName) ? product.SystemName : product.DisplayName,
                                              DisplayName = !string.IsNullOrWhiteSpace(product.DisplayName) ? product.DisplayName : product.SystemName,
                                              Client = new LookupItemDto<Guid>(product.ClientId, product.Client.SystemName),
                                              CreatedDate = product.CreationDate,
                                              EditedDate = product.ModificationDate,
                                          });

            sort = sort.HandleDefaultSorting(new string[] { "Url", "Name", "ClientId", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(filters, new string[] { "_Url", "_Name", "ClientId" }, "CreatedDate");

            query = query.OrderBy(sort);

            var pagedUsers = await query.ToPagedResultAsync(paginationInfo, cancellationToken);

            return pagedUsers;
        }



        public async Task<Result<List<Custom2LookupItemDto<Guid>>>> GetProductsLookupListAsync(string clientName, CancellationToken cancellationToken = default)
        {


            var products = await _dbContext.Products
                                           .Where(x => string.IsNullOrWhiteSpace(clientName) ||
                                                        clientName.ToLower().Equals(x.Client.SystemName))
                                            .AsNoTracking()
                                            .Select(x => new Custom2LookupItemDto<Guid>
                                            {
                                                Id = x.Id,
                                                Name = x.SystemName,
                                                DisplayName = x.DisplayName,
                                            })
                                            .ToListAsync(cancellationToken);

            return Result<List<Custom2LookupItemDto<Guid>>>.Successful(products);
        }

        public async Task<Result<List<ProductPublishedListItemDto>>> GetProductPublishedListAsync(string clientName, CancellationToken cancellationToken = default)
        {


            var products = await _dbContext.Products
                                           .Where(x => string.IsNullOrWhiteSpace(clientName) ||
                                                        clientName.ToLower().Equals(x.Client.SystemName))
                                            .AsNoTracking()
                                             .Select(product => new ProductPublishedListItemDto
                                             {
                                                 Id = product.Id,
                                                 Name = !string.IsNullOrWhiteSpace(product.SystemName) ? product.SystemName : product.DisplayName,
                                                 DisplayName = !string.IsNullOrWhiteSpace(product.DisplayName) ? product.DisplayName : product.SystemName,
                                                 Description = product.Description,
                                                 CreatedDate = product.CreationDate,
                                                 EditedDate = product.ModificationDate,
                                             })
                                            .ToListAsync(cancellationToken);

            return Result<List<ProductPublishedListItemDto>>.Successful(products);
        }





        public async Task<Result<List<CustomLookupItemDto<Guid>>>> GetProductsLookupListAsync(CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                                              .AsNoTracking()
                                              .Select(x => new CustomLookupItemDto<Guid>
                                              {
                                                  Id = x.Id,
                                                  Name = !string.IsNullOrWhiteSpace(x.SystemName) ? x.SystemName : x.DisplayName,
                                                  Title = x.DisplayName,
                                              })
                                              .ToListAsync(cancellationToken);

            return Result<List<CustomLookupItemDto<Guid>>>.Successful(products);
        }




        public async Task<Result<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products
                                          .AsNoTracking()
                                          .Where(x => x.Id == id)
                                          .Select(product => new ProductDto
                                          {
                                              Id = product.Id,
                                              DefaultHealthCheckUrl = product.DefaultHealthCheckUrl,
                                              HealthStatusChangeUrl = product.HealthStatusInformerUrl,
                                              Name = !string.IsNullOrWhiteSpace(product.SystemName) ? product.SystemName : product.DisplayName,
                                              DisplayName = !string.IsNullOrWhiteSpace(product.DisplayName) ? product.DisplayName : product.SystemName,
                                              Description = product.Description,
                                              IsPublished = product.IsPublished,
                                              Client = new LookupItemDto<Guid>(product.ClientId, product.Client.SystemName),
                                              CreatedDate = product.CreationDate,
                                              EditedDate = product.ModificationDate,
                                              ActivationEndpoint = product.ActivationUrl,
                                              CreationEndpoint = product.CreationUrl,
                                              DeactivationEndpoint = product.DeactivationUrl,
                                              DeletionEndpoint = product.DeletionUrl,
                                              ApiKey = product.ApiKey,
                                              SubscriptionResetUrl = product.SubscriptionResetUrl,
                                              SubscriptionUpgradeUrl = product.SubscriptionUpgradeUrl,
                                              SubscriptionDowngradeUrl = product.SubscriptionDowngradeUrl,
                                          })
                                          .SingleOrDefaultAsync(cancellationToken);


            var pro = new
            {
                product.ActivationEndpoint,
                product.CreationEndpoint,
                product.DeactivationEndpoint,
                product.DeletionEndpoint,
                product.DefaultHealthCheckUrl,
                product.HealthStatusChangeUrl,
                product.SubscriptionResetUrl,
                product.SubscriptionDowngradeUrl,
                product.SubscriptionUpgradeUrl,
                product.ApiKey,
            };
            var type = pro.GetType();
            var sddssd = type.GetProperties();

            product.WarningsNum = type.GetProperties()
                                      .Where(prop =>
            {
                var val = prop.GetValue(pro, null);
                return val == null || val.ToString() == string.Empty;
            }).Count();


            return Result<ProductDto>.Successful(product);
        }

        public async Task<Result<CreatedResult<Guid>>> CreateProductAsync(CreateProductModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreateProductValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            if (!await EnsureUniqueNameAsync(model.ClientId, model.Name))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.Name));
            }

            //if (!await EnsureUniqueUrlAsync(model.DefaultHealthCheckUrl))
            //{
            //    return Result<CreatedResult<Guid>>.Fail(ErrorMessage.UrlAlreadyExist, _identityContextService.Locale, nameof(model.DefaultHealthCheckUrl));
            //}
            #endregion


            var date = DateTime.UtcNow;

            var id = Guid.NewGuid();
            var product = new Product
            {
                Id = id,
                ClientId = model.ClientId,
                SystemName = model.Name,
                DisplayName = model.DisplayName,
                Description = model.Description,
                IsPublished = model.IsPublished,
                DefaultHealthCheckUrl = model.DefaultHealthCheckUrl,
                HealthStatusInformerUrl = model.HealthStatusChangeUrl,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                ActivationUrl = model.ActivationEndpoint,
                CreationUrl = model.CreationEndpoint,
                DeactivationUrl = model.DeactivationEndpoint,
                DeletionUrl = model.DeletionEndpoint,
                ApiKey = model.ApiKey,
                SubscriptionResetUrl = model.SubscriptionResetUrl,
                SubscriptionUpgradeUrl = model.SubscriptionUpgradeUrl,
                SubscriptionDowngradeUrl = model.SubscriptionDowngradeUrl,
                CreationDate = date,
                ModificationDate = date,
            };

            product.AddDomainEvent(new ProductCreatedEvent(product, _identityContextService.UserId, _identityContextService.GetUserType()));

            _dbContext.Products.Add(product);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(product.Id));
        }

        public async Task<Result> UpdateProductAsync(Guid id, UpdateProductModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdateProductValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            var product = await _dbContext.Products.Where(x => x.Id == id).SingleOrDefaultAsync(cancellationToken);
            if (product is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            //if (!await EnsureUniqueUrlAsync(model.Url, id, cancellationToken))
            //{
            //    return Result.Fail(ErrorMessage.UrlAlreadyExist, _identityContextService.Locale, nameof(model.Url));
            //}
            #endregion


            if (!product.DefaultHealthCheckUrl.Equals(model.DefaultHealthCheckUrl, StringComparison.OrdinalIgnoreCase))
            {
                var tenants = await _dbContext.Subscriptions.Where(x => x.ProductId == id && !x.HealthCheckUrlIsOverridden).ToListAsync(cancellationToken);
                foreach (var tenant in tenants)
                {
                    tenant.HealthCheckUrl = model.DefaultHealthCheckUrl;
                }
            }

            Product productBeforeUpdate = product.DeepCopy();

            product.DisplayName = model.DisplayName;
            product.Description = model.Description;
            product.HealthStatusInformerUrl = model.HealthStatusChangeUrl;
            product.DefaultHealthCheckUrl = model.DefaultHealthCheckUrl;
            product.ModifiedByUserId = _identityContextService.UserId;
            product.ActivationUrl = model.ActivationEndpoint;
            product.CreationUrl = model.CreationEndpoint;
            product.DeactivationUrl = model.DeactivationEndpoint;
            product.DeletionUrl = model.DeletionEndpoint;
            product.ApiKey = model.ApiKey;
            product.SubscriptionResetUrl = model.SubscriptionResetUrl;
            product.SubscriptionUpgradeUrl = model.SubscriptionUpgradeUrl;
            product.SubscriptionDowngradeUrl = model.SubscriptionDowngradeUrl;
            product.ModificationDate = DateTime.UtcNow;

            product.AddDomainEvent(new ProductUpdatedEvent(product, productBeforeUpdate));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
        {
            #region Validation 
            var product = await _dbContext.Products.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (product is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (!await DeletingIsAllowedAsync(id, cancellationToken))
            {
                return Result.Fail(ErrorMessage.DeletingIsNotAllowed, _identityContextService.Locale);
            }
            #endregion

            _dbContext.Products.Remove(product);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }




        public async Task<Result> PublishProductAsync(Guid id, PublishProductModel model, CancellationToken cancellationToken = default)
        {
            #region Validation 

            var product = await _dbContext.Products.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (product is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            #endregion 

            product.IsPublished = model.IsPublished;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }





        private async Task<bool> EnsureUniqueNameAsync(Guid clientId, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
        {
            return !await _dbContext.Products
                                    .Where(x => x.Id != id &&
                                               x.ClientId == clientId &&
                                                uniqueName.ToLower().Equals(x.SystemName))
                                    .AnyAsync(cancellationToken);
        }
        private async Task<bool> EnsureUniqueUrlAsync(string url, Guid id = new Guid(), CancellationToken cancellationToken = default)
        {
            return !await _dbContext.Tenants
                                    .Where(x => url.ToLower().Equals(x.SystemName) && x.Id != id)
                                    .AnyAsync(cancellationToken);
        }


        private async Task<bool> DeletingIsAllowedAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return !await _dbContext.Subscriptions
                                    .Include(x => x.Tenant)
                                    .Where(x => x.ProductId == productId &&
                                                x.Status != TenantStatus.Deleted)
                                    .AnyAsync(cancellationToken);

        }

        #endregion
    }
}
