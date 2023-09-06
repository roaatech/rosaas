using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.PlanFeatures.Models;
using Roaa.Rosas.Application.Services.Management.PlanFeatures.Validators;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanFeatures
{
    public class PlanFeatureService : IPlanFeatureService
    {
        #region Props 
        private readonly ILogger<PlanFeatureService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public PlanFeatureService(
            ILogger<PlanFeatureService> logger,
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
        public async Task<Result<List<PlanFeatureListItemDto>>> GetPlanFeaturesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var planFeatures = await _dbContext.PlanFeatures
                                              .AsNoTracking()
                                              .Where(f => f.Feature.ProductId == productId)
                                              .Select(planFeature => new PlanFeatureListItemDto
                                              {
                                                  Id = planFeature.Id,
                                                  Plan = new PlanItemDto
                                                  {
                                                      Id = planFeature.PlanId,
                                                      Name = planFeature.Plan.Name,
                                                      DisplayOrder = planFeature.Plan.DisplayOrder,
                                                  },
                                                  Limit = planFeature.Limit,
                                                  Unit = planFeature.Unit,
                                                  Description = planFeature.Description,
                                                  CreatedDate = planFeature.CreationDate,
                                                  EditedDate = planFeature.ModificationDate,
                                                  Feature = new FeatureItemDto
                                                  {
                                                      Id = planFeature.Feature.Id,
                                                      Name = planFeature.Feature.Name,
                                                      Type = planFeature.Feature.Type,
                                                  },
                                              })
                                              .OrderByDescending(x => x.Plan.DisplayOrder)
                                              .ToListAsync(cancellationToken);

            var plansIds = planFeatures.Select(x => x.Plan.Id).ToList();
            var plans = await _dbContext.Plans
                              .AsNoTracking()
                               .Where(p => p.ProductId == productId &&
                                           !plansIds.Contains(p.Id))
                               .Select(plan => new PlanFeatureListItemDto
                               {
                                   Plan = new PlanItemDto
                                   {
                                       Id = plan.Id,
                                       Name = plan.Name,
                                       DisplayOrder = plan.DisplayOrder,
                                   },
                               })
                               .ToListAsync(cancellationToken);
            if (plans.Any())
            {
                planFeatures.AddRange(plans);
                planFeatures = planFeatures.OrderBy(x => x.Plan.DisplayOrder).ToList();
            }




            return Result<List<PlanFeatureListItemDto>>.Successful(planFeatures);
        }

        public async Task<Result<CreatedResult<Guid>>> CreatePlanFeatureAsync(CreatePlanFeatureModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreatePlanFeatureValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            if (await _dbContext.PlanFeatures.Where(x => x.FeatureId == model.FeatureId && x.PlanId == model.PlanId).AnyAsync())
            {
                return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.ResourceAlreadyExists, _identityContextService.Locale);
            }
            #endregion

            var date = DateTime.UtcNow;

            var planFeature = new PlanFeature
            {
                Id = Guid.NewGuid(),
                FeatureId = model.FeatureId,
                PlanId = model.PlanId,
                Limit = model.Limit,
                Unit = model.Unit,
                Description = model.Description,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.PlanFeatures.Add(planFeature);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(planFeature.Id));
        }

        public async Task<Result> UpdatePlanFeatureAsync(Guid planFeatureId, UpdatePlanFeatureModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdatePlanFeatureValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            var planFeature = await _dbContext.PlanFeatures.Where(x => x.Id == planFeatureId).SingleOrDefaultAsync();
            if (planFeature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (!await UpdatingOrDeletingIsAllowedAsync(planFeatureId, cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion
            PlanFeature featureBeforeUpdate = planFeature.DeepCopy();

            planFeature.Limit = model.Limit;
            planFeature.Unit = model.Unit;
            planFeature.Description = model.Description;
            planFeature.ModifiedByUserId = _identityContextService.UserId;
            planFeature.ModificationDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeletePlanFeatureAsync(Guid planFeatureId, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 
            var feature = await _dbContext.PlanFeatures.Where(x => x.Id == planFeatureId).SingleOrDefaultAsync();
            if (feature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (!await UpdatingOrDeletingIsAllowedAsync(planFeatureId, cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion

            _dbContext.PlanFeatures.Remove(feature);

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
