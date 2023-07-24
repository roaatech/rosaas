using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Products
{
    public partial class ProductService : IProductService
    {
        #region Props 
        private readonly ILogger<ProductService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public ProductService(
            ILogger<ProductService> logger,
            IRosasDbContext dbContext,
            IWebHostEnvironment environment,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _environment = environment;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services   


        public async Task<Result<List<ProductUrlListItem>>> GetProductsUrlsByTenantIdAsync(Guid tenantId, Expression<Func<ProductTenant, ProductUrlListItem>> selector, CancellationToken cancellationToken = default)
        {
            var urls = await _dbContext.ProductTenants.AsNoTracking()
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
                                              Url = product.DefaultHealthCheckUrl,
                                              Name = product.Name,
                                              Client = new LookupItemDto<Guid>(product.ClientId, product.Client.UniqueName),
                                              CreatedDate = product.Created,
                                              EditedDate = product.Edited,
                                          });

            sort = sort.HandleDefaultSorting(new string[] { "Url", "Name", "ClientId", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(filters, new string[] { "_Url", "_Name", "ClientId" }, "CreatedDate");

            query = query.OrderBy(sort);

            var pagedUsers = await query.ToPagedResultAsync(paginationInfo, cancellationToken);

            return pagedUsers;
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products
                                          .AsNoTracking()
                                          .Include(x => x.Client)
                                          .Where(x => x.Id == id)
                                          .Select(product => new ProductDto
                                          {
                                              Id = product.Id,
                                              DefaultHealthCheckUrl = product.DefaultHealthCheckUrl,
                                              HealthStatusChangeUrl = product.HealthStatusChangeUrl,
                                              Name = product.Name,
                                              Client = new LookupItemDto<Guid>(product.ClientId, product.Client.UniqueName),
                                              CreatedDate = product.Created,
                                              EditedDate = product.Edited,
                                              ActivationEndpoint = product.ActivationUrl,
                                              CreationEndpoint = product.CreationUrl,
                                              DeactivationEndpoint = product.DeactivationUrl,
                                              DeletionEndpoint = product.DeletionUrl,
                                          })
                                          .SingleOrDefaultAsync(cancellationToken);

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

            if (!await EnsureUniqueUrlAsync(model.DefaultHealthCheckUrl))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.UrlAlreadyExist, _identityContextService.Locale, nameof(model.DefaultHealthCheckUrl));
            }
            #endregion


            var date = DateTime.UtcNow;

            var id = Guid.NewGuid();
            var product = new Product
            {
                Id = id,
                ClientId = model.ClientId,
                Name = model.Name,
                DefaultHealthCheckUrl = model.DefaultHealthCheckUrl,
                HealthStatusChangeUrl = model.HealthStatusChangeUrl,
                CreatedByUserId = _identityContextService.UserId,
                EditedByUserId = _identityContextService.UserId,
                ActivationUrl = model.ActivationEndpoint,
                CreationUrl = model.CreationEndpoint,
                DeactivationUrl = model.DeactivationEndpoint,
                DeletionUrl = model.DeletionEndpoint,
                Created = date,
                Edited = date,
            };

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

            if (!await EnsureUniqueUrlAsync(model.Url, id, cancellationToken))
            {
                return Result.Fail(ErrorMessage.UrlAlreadyExist, _identityContextService.Locale, nameof(model.Url));
            }
            #endregion


            if (!product.DefaultHealthCheckUrl.Equals(model.DefaultHealthCheckUrl, StringComparison.OrdinalIgnoreCase))
            {
                var tenants = await _dbContext.ProductTenants.Where(x => x.ProductId == id && !x.HealthCheckUrlIsOverridden).ToListAsync(cancellationToken);
                foreach (var tenant in tenants)
                {
                    tenant.HealthCheckUrl = model.DefaultHealthCheckUrl;
                }
            }

            Product productBeforeUpdate = product.DeepCopy();

            product.Name = model.Name;
            product.HealthStatusChangeUrl = model.HealthStatusChangeUrl;
            product.DefaultHealthCheckUrl = model.DefaultHealthCheckUrl;
            product.EditedByUserId = _identityContextService.UserId;
            product.ActivationUrl = model.ActivationEndpoint;
            product.CreationUrl = model.CreationEndpoint;
            product.DeactivationUrl = model.DeactivationEndpoint;
            product.DeletionUrl = model.DeletionEndpoint;
            product.Edited = DateTime.UtcNow;

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

        private async Task<bool> EnsureUniqueUrlAsync(string url, Guid id = new Guid(), CancellationToken cancellationToken = default)
        {
            return !await _dbContext.Tenants
                                    .Where(x => url.ToLower().Equals(x.UniqueName) && x.Id != id)
                                    .AnyAsync(cancellationToken);
        }


        private async Task<bool> DeletingIsAllowedAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return !await _dbContext.ProductTenants
                                    .Include(x => x.Tenant)
                                    .Where(x => x.ProductId == productId &&
                                                x.Status != TenantStatus.Deleted)
                                    .AnyAsync(cancellationToken);

        }

        #endregion
    }
}
