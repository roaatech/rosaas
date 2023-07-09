using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Features.Models;
using Roaa.Rosas.Application.Services.Management.Features.Validators;
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
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public FeatureService(
            ILogger<FeatureService> logger,
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
        public async Task<PaginatedResult<FeatureListItemDto>> GetFeaturesPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Features.AsNoTracking();

            sort = sort.HandleDefaultSorting(new string[] { "Description", "Name", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(filters, new string[] { "_Description", "_Name", "ProductId" }, "CreatedDate");

            var selectedQuery = query.Select(feature => new FeatureListItemDto
            {
                Id = feature.Id,
                Name = feature.Name,
                Description = feature.Description,
                Reset = feature.Reset,
                Type = feature.Type,
                Unit = feature.Unit,
                CreatedDate = feature.Created,
                EditedDate = feature.Edited,
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
                                                  Description = feature.Description,
                                                  Reset = feature.Reset,
                                                  Type = feature.Type,
                                                  Unit = feature.Unit,
                                                  CreatedDate = feature.Created,
                                                  EditedDate = feature.Edited,
                                              })
                                              .ToListAsync(cancellationToken);

            return Result<List<FeatureListItemDto>>.Successful(features);
        }


        public async Task<Result<FeatureDto>> GetFeatureByIdAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            var feature = await _dbContext.Features
                                          .AsNoTracking()
                                          .Where(x => x.Id == id)
                                          .Select(feature => new FeatureDto
                                          {
                                              Id = feature.Id,
                                              Name = feature.Name,
                                              Description = feature.Description,
                                              Reset = feature.Reset,
                                              Type = feature.Type,
                                              Unit = feature.Unit,
                                              CreatedDate = feature.Created,
                                              EditedDate = feature.Edited,
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

            #endregion


            var date = DateTime.UtcNow;

            var id = Guid.NewGuid();
            var feature = new Feature
            {
                Id = id,
                Name = model.Name,
                Description = model.Description,
                Reset = model.Reset,
                Type = model.Type,
                Unit = model.Unit,
                CreatedByUserId = _identityContextService.UserId,
                EditedByUserId = _identityContextService.UserId,
                Created = date,
                Edited = date,
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

            var feature = await _dbContext.Features.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (feature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (!await UpdatingOrDeletingIsAllowedAsync(id, cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion
            Feature featureBeforeUpdate = feature.DeepCopy();

            feature.Name = model.Name;
            feature.Description = model.Description;
            feature.Reset = model.Reset;
            feature.Type = model.Type;
            feature.Unit = model.Unit;
            feature.EditedByUserId = _identityContextService.UserId;
            feature.Edited = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeleteFeatureAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 

            var feature = await _dbContext.Features.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (feature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (!await UpdatingOrDeletingIsAllowedAsync(id, cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion

            _dbContext.Features.Remove(feature);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        private async Task<bool> UpdatingOrDeletingIsAllowedAsync(Guid featureId, CancellationToken cancellationToken = default)
        {
            return true;
        }
        #endregion
    }
}
