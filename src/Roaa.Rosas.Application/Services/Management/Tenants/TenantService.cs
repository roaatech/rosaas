using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Validators;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public partial class TenantService : ITenantService
    {
        #region Props 
        private readonly ILogger<TenantService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public TenantService(
            ILogger<TenantService> logger,
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

        public async Task<PaginatedResult<TenantListItemDto>> GetTenantsPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Tenants.AsNoTracking()
                                          .Select(tenant => new TenantListItemDto
                                          {
                                              Id = tenant.Id,
                                              UniqueName = tenant.UniqueName,
                                              Title = tenant.Title,
                                              ProductId = tenant.ProductId,
                                              Status = tenant.Status,
                                              CreatedDate = tenant.Created,
                                              EditedDate = tenant.Edited,
                                              ProductName = tenant.Product.Title,
                                          });

            sort = sort.HandleDefaultSorting(new string[] { "UniqueName", "Title", "ProductId" }, "EditedDate", SortDirection.Desc);

            query = query.Where(filters, new string[] { "_UniqueName", "_Title", "ProductId" }, "CreatedDate");

            query = query.OrderBy(sort);

            var pagedUsers = await query.ToPagedResultAsync(paginationInfo, cancellationToken);

            return pagedUsers;
        }

        public async Task<Result<TenantDto>> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var tenant = await _dbContext.Tenants.AsNoTracking()
                                                   .Include(x => x.Product)
                                                   .Where(x => x.Id == id)
                                                   .Select(tenant => new TenantDto
                                                   {
                                                       Id = tenant.Id,
                                                       UniqueName = tenant.UniqueName,
                                                       Title = tenant.Title,
                                                       ProductId = tenant.ProductId,
                                                       Status = tenant.Status,
                                                       CreatedDate = tenant.Created,
                                                       EditedDate = tenant.Edited,
                                                       ProductName = tenant.Product.Title,
                                                   })
                                                   .SingleOrDefaultAsync(cancellationToken);

            return Result<TenantDto>.Successful(tenant);
        }

        public async Task<Result<CreatedResult<Guid>>> CreateTenantAsync(CreateTenantModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreateTenantValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            if (!await EnsureUniqueNameAsync(model.ProductId, model.UniqueName))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.UniqueName));
            }
            #endregion


            var date = DateTime.UtcNow;

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                ProductId = model.ProductId,
                UniqueName = model.UniqueName.ToLower(),
                Title = model.Title,
                Status = TenantStatus.Active,
                CreatedByUserId = _identityContextService.UserId,
                EditedByUserId = _identityContextService.UserId,
                Created = date,
                Edited = date,
            };

            tenant.AddDomainEvent(new TenantCreatedEvent(tenant));

            _dbContext.Tenants.Add(tenant);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(tenant.Id));
        }

        public async Task<Result> UpdateTenantAsync(UpdateTenantModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdateTenantValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            var tenant = await _dbContext.Tenants.Where(x => x.Id == model.Id).SingleOrDefaultAsync();
            if (tenant is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (!await EnsureUniqueNameAsync(tenant.ProductId, model.UniqueName, model.Id))
            {
                return Result.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.UniqueName));
            }
            #endregion

            Tenant tenantBeforeUpdate = tenant.DeepCopy();

            tenant.UniqueName = model.UniqueName.ToLower();
            tenant.Title = model.Title;
            tenant.EditedByUserId = _identityContextService.UserId;
            tenant.Edited = DateTime.UtcNow;

            tenant.AddDomainEvent(new TenantUpdatedEvent(tenantBeforeUpdate, tenant));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        private async Task<bool> EnsureUniqueNameAsync(Guid productId, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
        {
            return !await _dbContext.Tenants.Where(x => x.ProductId == productId &&
                                                        uniqueName.ToLower().Equals(x.UniqueName) &&
                                                        x.Id != id)
                                           .AnyAsync(cancellationToken);
        }

        #endregion
    }
}
