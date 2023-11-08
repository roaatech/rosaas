using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Features.Models;
using Roaa.Rosas.Application.Services.Management.Features.Validators;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Features
{
    public class FeatureService : IFeatureService
    {
        #region Props 
        private readonly ILogger<FeatureService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public FeatureService(
            ILogger<FeatureService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services    
        public async Task<PaginatedResult<FeatureListItemDto>> GetFeaturesPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Features.AsNoTracking();

            sort = sort.HandleDefaultSorting(new string[] { "Description", "Name", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(filters, new string[] { "_Description", "_Name", "ProductId" }, "CreatedDate");

            var selectedQuery = query.Select(feature => new FeatureListItemDto
            {
                Id = feature.Id,
                Name = feature.Name,
                Title = feature.DisplayName,
                Description = feature.Description,
                Reset = feature.Reset,
                Type = feature.Type,
                CreatedDate = feature.CreationDate,
                EditedDate = feature.ModificationDate,
            });

            selectedQuery = selectedQuery.OrderBy(sort);

            var pagedUsers = await selectedQuery.ToPagedResultAsync(paginationInfo, cancellationToken);

            return pagedUsers;
        }


        public async Task<Result<List<FeatureListItemDto>>> GetFeaturesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var features = await _dbContext.Features
                                              .AsNoTracking()
                                              .Where(f => f.ProductId == productId)
                                              .Select(feature => new FeatureListItemDto
                                              {
                                                  Id = feature.Id,
                                                  Name = feature.Name,
                                                  Title = feature.DisplayName,
                                                  Description = feature.Description,
                                                  Reset = feature.Reset,
                                                  Type = feature.Type,
                                                  CreatedDate = feature.CreationDate,
                                                  EditedDate = feature.ModificationDate,
                                                  IsSubscribed = feature.IsSubscribed,
                                              })
                                              .OrderByDescending(x => x.EditedDate)
                                              .ToListAsync(cancellationToken);

            return Result<List<FeatureListItemDto>>.Successful(features);
        }

        public async Task<Result<List<LookupItemDto<Guid>>>> GetFeaturesLookupListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var features = await _dbContext.Features
                                              .AsNoTracking()
                                              .Where(f => f.ProductId == productId)
                                              .Select(feature => new LookupItemDto<Guid>
                                              {
                                                  Id = feature.Id,
                                                  Name = feature.DisplayName,
                                              })
                                               .OrderBy(x => x.Name)
                                              .ToListAsync(cancellationToken);

            return Result<List<LookupItemDto<Guid>>>.Successful(features);
        }


        public async Task<Result<FeatureDto>> GetFeatureByIdAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            var feature = await _dbContext.Features
                                          .AsNoTracking()
                                          .Where(x => x.Id == id && x.ProductId == productId)
                                          .Select(feature => new FeatureDto
                                          {
                                              Id = feature.Id,
                                              Name = feature.Name,
                                              Title = feature.DisplayName,
                                              Description = feature.Description,
                                              Reset = feature.Reset,
                                              Type = feature.Type,
                                              CreatedDate = feature.CreationDate,
                                              EditedDate = feature.ModificationDate,
                                              IsSubscribed = feature.IsSubscribed,
                                          })
                                          .SingleOrDefaultAsync(cancellationToken);

            return Result<FeatureDto>.Successful(feature);
        }

        public async Task<Result<CreatedResult<Guid>>> CreateFeatureAsync(CreateFeatureModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreateFeatureValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            if (!await _dbContext.Products.Where(x => x.Id == productId).AnyAsync(cancellationToken))
            {
                return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, "productId");
            }

            if (!await EnsureUniqueNameAsync(productId, model.Name))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.Name));
            }
            #endregion


            var date = DateTime.UtcNow;

            var id = Guid.NewGuid();
            var feature = new Feature
            {
                Id = id,
                ProductId = productId,
                Name = model.Name,
                DisplayName = model.Title,
                Description = model.Description,
                Reset = model.Reset,
                Type = model.Type,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.Features.Add(feature);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(feature.Id));
        }

        public async Task<Result> UpdateFeatureAsync(Guid id, UpdateFeatureModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdateFeatureValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            var feature = await _dbContext.Features.Where(x => x.Id == id && x.ProductId == productId).SingleOrDefaultAsync();
            if (feature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (feature.IsSubscribed)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale);
            }

            if (!await EnsureUniqueNameAsync(productId, model.Name, id))
            {
                return Result.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.Name));
            }

            #endregion
            Feature featureBeforeUpdate = feature.DeepCopy();

            feature.Name = model.Name;
            feature.DisplayName = model.Title;
            feature.Description = model.Description;
            feature.Reset = model.Reset;
            feature.Type = model.Type;
            feature.ModifiedByUserId = _identityContextService.UserId;
            feature.ModificationDate = DateTime.UtcNow;


            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeleteFeatureAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 

            var feature = await _dbContext.Features.Where(x => x.Id == id && x.ProductId == productId).SingleOrDefaultAsync();
            if (feature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (feature.IsSubscribed)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale);
            }
            #endregion 

            var planFeatures = await _dbContext.PlanFeatures.Where(x => x.FeatureId == id).ToListAsync(cancellationToken);
            if (planFeatures.Any())
            {
                _dbContext.PlanFeatures.RemoveRange(planFeatures);
            }

            _dbContext.Features.Remove(feature);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }
        #endregion




        private async Task<bool> EnsureUniqueNameAsync(Guid productId, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
        {
            return !await _dbContext.Features
                                    .Where(x => x.Id != id &&
                                               x.ProductId == productId &&
                                                uniqueName.ToLower().Equals(x.Name))
                                    .AnyAsync(cancellationToken);
        }
    }
}
