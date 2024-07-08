using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.ProductOwner;
using Roaa.Rosas.Application.Services.Management.ProductOwners.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.ProductOwners
{
    public class ProductOwnerService : IProductOwnerService
    {
        private readonly ILogger<ProductOwnerService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;

        #region Corts
        public ProductOwnerService(ILogger<ProductOwnerService> logger, IRosasDbContext dbContext, IIdentityContextService identityContextService)

        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }
        #endregion

        #region Services 
        public async Task<Result<List<ProductOwnerListItemDto>>> GetAllProductOwnersAsync(CancellationToken cancellationToken = default)
        {
            var productOwners = await _dbContext.ProductOwners
                                                .AsNoTracking()
                                                .Select(po => new ProductOwnerListItemDto
                                                {
                                                    Id = po.Id,
                                                    SystemName = po.SystemName,
                                                    DisplayName = po.DisplayName,
                                                    IsDeleted = po.IsDeleted,
                                                })
                                                .ToListAsync(cancellationToken);

            return Result<List<ProductOwnerListItemDto>>.Successful(productOwners);
        }

        public async Task<Result<ProductOwnerDto>> GetProductOwnerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var products = _dbContext.Products
                                                                        .Where(p => p.ClientId == id)
                                                                        .Select(p => new CustomLookupItemDto<Guid>
                                                                        {
                                                                            Id = p.Id,
                                                                            SystemName = p.SystemName,
                                                                            DisplayName = p.DisplayName
                                                                        })
                                                                        .ToList();
            var productOwner = await _dbContext.ProductOwners
                                                .AsNoTracking()
                                                .Where(po => po.Id == id)
                                                .Select(po => new ProductOwnerDto
                                                {
                                                    Id = po.Id,
                                                    SystemName = po.SystemName,
                                                    DisplayName = po.DisplayName,
                                                    IsDeleted = po.IsDeleted,
                                                    CreatedDate = po.CreationDate,
                                                    EditedDate = po.ModificationDate,
                                                    Products = products

                                                })


                                                .SingleOrDefaultAsync(cancellationToken);

            if (productOwner == null)
            {
                return Result<ProductOwnerDto>.Fail($"ProductOwner with ID {id} not found.");
            }
            return Result<ProductOwnerDto>.Successful(productOwner);
        }

        public async Task<Result<CreatedResult<Guid>>> CreateProductOwnerAsync(CreateProductOwnerModel productOwnerModel, CancellationToken cancellationToken = default)
        {
            var id = Guid.NewGuid();
            var date = DateTime.UtcNow;

            var productOwner = new Domain.Entities.Management.ProductOwner
            {
                Id = id,
                SystemName = productOwnerModel.SystemName,
                DisplayName = productOwnerModel.DisplayName,
                Description = productOwnerModel.Description,
                IsDeleted = false,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.ProductOwners.Add(productOwner);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(productOwner.Id));
        }



        public async Task<Result> UpdateProductOwnerAsync(Guid id, UpdateProductOwnerModel productOwnerModel, CancellationToken cancellationToken = default)
        {
            var existingProductOwner = await _dbContext.ProductOwners.FindAsync(id);

            if (existingProductOwner == null)
            {
                return Result.Fail($"ProductOwner with ID {id} not found.");
            }

            existingProductOwner.SystemName = productOwnerModel.SystemName;
            existingProductOwner.DisplayName = productOwnerModel.DisplayName;
            existingProductOwner.Description = productOwnerModel.Description;
            existingProductOwner.IsDeleted = productOwnerModel.IsDeleted;
            existingProductOwner.ModifiedByUserId = _identityContextService.UserId;
            existingProductOwner.ModificationDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }
        public async Task<PaginatedResult<ProductOwnerListItemDto>> GetProductOwnersPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductOwners
                                  .AsNoTracking()
                                  .Include(po => po.Products)
                                  .Select(po => new ProductOwnerListItemDto
                                  {
                                      Id = po.Id,
                                      SystemName = po.SystemName,
                                      DisplayName = po.DisplayName,
                                      IsDeleted = po.IsDeleted,
                                      CreatedDate = po.CreationDate,
                                      EditedDate = po.ModificationDate,

                                  });

            sort = sort.HandleDefaultSorting(new string[] { "SystemName", "DisplayName", "IsDeleted" }, "SystemName", SortDirection.Asc);

            query = query.Where(filters, new string[] { "_SystemName", "_DisplayName", "IsDeleted" });

            query = query.OrderBy(sort);

            var pagedProductOwners = await query.ToPagedResultAsync(paginationInfo, cancellationToken);

            return pagedProductOwners;
        }

        public async Task<Result> DeleteProductOwnerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var productOwner = await _dbContext.ProductOwners.FindAsync(id);

            if (productOwner == null)
            {
                return Result.Fail($"ProductOwner with ID {id} not found.");
            }

            productOwner.IsDeleted = true;
            productOwner.ModificationDate = DateTime.UtcNow;
            productOwner.ModifiedByUserId = _identityContextService.UserId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }
        public async Task<Result<ProductOwnerRegistrationStatusDto>> IsProductOwnerRegisteredAsync(CancellationToken cancellationToken = default)
        {
            var userId = _identityContextService.UserId;

            var isRegistered = await _dbContext.ProductOwners
                                               .AsNoTracking()
                                               .AnyAsync(po => po.CreatedByUserId == userId, cancellationToken);

            var result = new ProductOwnerRegistrationStatusDto
            {
                IsProductOwnerRegistered = isRegistered
            };

            return Result<ProductOwnerRegistrationStatusDto>.Successful(result);
        }
        #endregion
    }
}
