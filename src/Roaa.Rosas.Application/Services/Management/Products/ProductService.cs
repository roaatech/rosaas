using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;

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


        public async Task<Result<List<ProductUrlListItem>>> GetProductsUrlsByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            var urls = await _dbContext.ProductTenants.AsNoTracking()
                                                  .Include(x => x.Product)
                                                   .Where(x => x.TenantId == tenantId)
                                                   .Select(x => new ProductUrlListItem
                                                   {
                                                       Id = x.TenantId,
                                                       Url = x.Product.Url,
                                                   })
                                                   .ToListAsync(cancellationToken);

            return Result<List<ProductUrlListItem>>.Successful(urls);
        }
        #endregion
    }
}
