using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.PlanFeatures.Models;
using Roaa.Rosas.Application.Services.Management.PlanFeatures.Validators;
using Roaa.Rosas.Application.SystemMessages;
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
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public PlanFeatureService(
            ILogger<PlanFeatureService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
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
                                                      Title = planFeature.Plan.DisplayName,
                                                      DisplayOrder = planFeature.Plan.DisplayOrder,
                                                      IsPublished = planFeature.Plan.IsPublished,
                                                      IsSubscribed = planFeature.Plan.IsSubscribed,
                                                  },
                                                  Limit = planFeature.Limit,
                                                  Unit = planFeature.FeatureUnit,
                                                  Description = planFeature.Description,
                                                  CreatedDate = planFeature.CreationDate,
                                                  EditedDate = planFeature.ModificationDate,
                                                  Feature = new FeatureItemDto
                                                  {
                                                      Id = planFeature.Feature.Id,
                                                      Name = planFeature.Feature.Name,
                                                      Title = planFeature.Feature.DisplayName,
                                                      Type = planFeature.Feature.Type,
                                                      IsSubscribed = planFeature.Feature.IsSubscribed,
                                                      Reset = planFeature.Feature.Reset,
                                                  },
                                              })
                                              .OrderBy(x => x.Plan.DisplayOrder)
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
                                       Title = plan.DisplayName,
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

            if (!await _dbContext.Features.Where(x => x.Id == model.FeatureId && x.ProductId == productId).AnyAsync())
            {
                return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale, nameof(model.FeatureId));
            }

            var plan = await _dbContext.Plans.Where(x => x.Id == model.PlanId && x.ProductId == productId).SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale, nameof(model.PlanId));
            }

            if (plan.IsSubscribed)
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale, nameof(model.PlanId));
            }
            #endregion

            var date = DateTime.UtcNow;

            var planFeature = new PlanFeature
            {
                Id = Guid.NewGuid(),
                FeatureId = model.FeatureId,
                PlanId = model.PlanId,
                Limit = model.Limit,
                FeatureUnit = model.Unit,
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

            var planFeature = await _dbContext.PlanFeatures
                                              .Include(x => x.Plan)
                                              .Where(x => x.Id == planFeatureId)
                                              .SingleOrDefaultAsync();
            if (planFeature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (planFeature.Plan.IsSubscribed)
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale, "Plan");
            }


            #endregion
            PlanFeature featureBeforeUpdate = planFeature.DeepCopy();

            planFeature.Limit = model.Limit;
            planFeature.FeatureUnit = model.Unit;
            planFeature.Description = model.Description;
            planFeature.ModifiedByUserId = _identityContextService.UserId;
            planFeature.ModificationDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        public async Task<Result> DeletePlanFeatureAsync(Guid planFeatureId, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 
            var planFeature = await _dbContext.PlanFeatures
                                              .Include(x => x.Plan)
                                              .Where(x => x.Id == planFeatureId)
                                              .SingleOrDefaultAsync();
            if (planFeature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }


            if (planFeature.Plan.IsSubscribed)
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale, "Plan");
            }

            #endregion

            _dbContext.PlanFeatures.Remove(planFeature);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        #endregion
    }
}
