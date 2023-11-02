using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Specifications.Models;
using Roaa.Rosas.Application.Services.Management.Specifications.Validators;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Specifications
{
    public class SpecificationService : ISpecificationService
    {
        #region Props 
        private readonly ILogger<SpecificationService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public SpecificationService(
            ILogger<SpecificationService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services    



        public async Task<Result<List<SpecificationListItemDto>>> GetSpecificationsListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var fields = await _dbContext.Specifications
                                            .AsNoTracking()
                                            .Where(f => f.ProductId == productId)
                                            .Select(field => new SpecificationListItemDto
                                            {
                                                Id = field.Id,
                                                DisplayName = field.DisplayName,
                                                Description = field.Description,
                                                Name = field.Name,
                                                DataType = field.DataType,
                                                InputType = field.InputType,
                                                IsRequired = field.IsRequired,
                                                IsUserEditable = field.IsUserEditable,
                                                ValidationFailureDescription = field.ValidationFailureDescription,
                                                RegularExpression = field.RegularExpression,
                                                CreatedDate = field.CreationDate,
                                                EditedDate = field.ModificationDate,
                                                IsSubscribed = field.IsSubscribed,
                                                IsPublished = field.IsPublished,
                                            })
                                            .OrderByDescending(x => x.EditedDate)
                                            .ToListAsync(cancellationToken);

            return Result<List<SpecificationListItemDto>>.Successful(fields);
        }


        public async Task<Result<CreatedResult<Guid>>> CreateSpecificationAsync(Guid productId, CreateSpecificationModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreateSpecificationValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            if (!await EnsureUniqueNameAsync(productId, model.Name, null, cancellationToken))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.Name));
            }
            #endregion

            var date = DateTime.UtcNow;

            var id = Guid.NewGuid();

            var field = new Specification
            {
                Id = id,
                ProductId = productId,
                Name = model.Name,
                NormalizedName = model.Name.ToUpper(),
                DisplayName = model.DisplayName,
                Description = model.Description,
                DataType = model.DataType,
                InputType = model.InputType,
                IsRequired = model.IsRequired,
                IsUserEditable = model.IsUserEditable,
                IsPublished = model.IsPublished,
                ValidationFailureDescription = model.ValidationFailureDescription,
                RegularExpression = model.RegularExpression,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.Specifications.Add(field);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(field.Id));
        }


        public async Task<Result> UpdateSpecificationAsync(Guid id, Guid productId, UpdateSpecificationModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdateSpecificationValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            if (!await EnsureUniqueNameAsync(productId, model.Name, id, cancellationToken))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.Name));
            }

            var field = await _dbContext.Specifications.Where(x => x.Id == id && x.ProductId == productId).SingleOrDefaultAsync();
            if (field is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (field.IsSubscribed)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale);
            }
            #endregion

            Specification fieldBeforeUpdate = field.DeepCopy();

            field.Name = model.Name;
            field.NormalizedName = model.Name.ToUpper();
            field.DisplayName = model.DisplayName;
            field.Description = model.Description;
            field.DataType = model.DataType;
            field.InputType = model.InputType;
            field.IsRequired = model.IsRequired;
            field.IsUserEditable = model.IsUserEditable;
            field.ValidationFailureDescription = model.ValidationFailureDescription;
            field.RegularExpression = model.RegularExpression;
            field.ModifiedByUserId = _identityContextService.UserId;
            field.ModificationDate = DateTime.UtcNow;
            if (model.IsPublished.HasValue)
            {

                field.IsPublished = model.IsPublished.Value;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeleteSpecificationAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 

            var field = await _dbContext.Specifications.Where(x => x.Id == id && x.ProductId == productId).SingleOrDefaultAsync();
            if (field is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (field.IsSubscribed)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale);
            }
            #endregion 

            _dbContext.Specifications.Remove(field);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public async Task<Result> PublishSpecificationAsync(Guid id, Guid productId, PublishSpecificationModel model, CancellationToken cancellationToken = default)
        {
            #region Validation  
            var field = await _dbContext.Specifications.Where(x => x.Id == id && x.ProductId == productId).SingleOrDefaultAsync();
            if (field is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }
            #endregion

            field.IsPublished = model.IsPublished;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        public async Task<Result> SetSpecificationsAsSubscribedAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            var specificationIds = await _dbContext.SpecificationValues
                                        .Where(x => x.TenantId == tenantId)
                                        .Select(x => x.SpecificationId)
                                        .ToListAsync();

            return await SetSpecificationsAsSubscribedAsync(specificationIds, cancellationToken);
        }
        public async Task<Result> SetSpecificationsAsSubscribedAsync(List<Guid> ids, CancellationToken cancellationToken = default)
        {
            var specifications = await _dbContext.Specifications
                                        .Where(x => ids.Contains(x.Id) &&
                                                   !x.IsSubscribed
                                                    )
                                        .ToListAsync();
            if (specifications.Any())
            {
                foreach (var specification in specifications)
                {
                    specification.IsSubscribed = true;
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Successful();
        }


        private async Task<bool> EnsureUniqueNameAsync(Guid productId, string name, Guid? id, CancellationToken cancellationToken = default)
        {
            id = id ?? Guid.NewGuid();
            return !await _dbContext.Specifications
                                    .Where(x => x.Id != id &&
                                                x.ProductId == productId &&
                                                name.ToUpper().Equals(x.NormalizedName))
                                    .AnyAsync(cancellationToken);
        }
        #endregion


        /*
    public async Task<Result<List<LookupItemDto<Guid>>>> GetSpecificationsLookupListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var fields = await _dbContext.Specifications
                                          .AsNoTracking()
                                          .Where(f => f.ProductId == productId)
                                          .Select(field => new LookupItemDto<Guid>
                                          {
                                              Id = field.Id,
                                              Name = field.Name,
                                          })
                                           .OrderBy(x => x.Name)
                                          .ToListAsync(cancellationToken);

        return Result<List<LookupItemDto<Guid>>>.Successful(fields);
    }

    public async Task<Result<SpecificationDto>> GetSpecificationByIdAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
    {
        var field = await _dbContext.Specifications
                                      .AsNoTracking()
                                      .Where(x => x.Id == id && x.ProductId == productId)
                                      .Select(field => new SpecificationDto
                                      {
                                          Id = field.Id,
                                          Name = field.Name,
                                          Description = field.Description,
                                          Reset = field.Reset,
                                          Type = field.Type,
                                          CreatedDate = field.CreationDate,
                                          EditedDate = field.ModificationDate,
                                          IsSubscribed = field.IsSubscribed,
                                      })
                                      .SingleOrDefaultAsync(cancellationToken);

        return Result<SpecificationDto>.Successful(field);
    }
    */
    }
}
