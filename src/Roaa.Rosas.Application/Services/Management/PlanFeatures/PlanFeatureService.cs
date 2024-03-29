﻿using FluentValidation;
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
using System.Linq.Expressions;

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

        public async Task<Result<List<PlanFeatureListItemDto>>> GetPublishedPlanFeaturesListByProductNameAsync(string productName, CancellationToken cancellationToken = default)
        {
            var planFeatures = await _dbContext.PlanFeatures
                                              .AsNoTracking()
                                              .Where(pf => productName.ToLower().Equals(pf.Plan.Product.SystemName) && pf.Plan.IsPublished)
                                              .Select(GetPlanFeatureListItemDtoSelector())
                                              .OrderBy(x => x.Plan.DisplayOrder)
                                              .ToListAsync(cancellationToken);






            return Result<List<PlanFeatureListItemDto>>.Successful(planFeatures);
        }

        public async Task<Result<List<PlanFeatureListItemDto>>> GetPlanFeaturesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var planFeatures = await _dbContext.PlanFeatures
                                              .AsNoTracking()
                                              .Where(f => f.Feature.ProductId == productId)
                                              .Select(GetPlanFeatureListItemDtoSelector())
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
                                       SystemName = plan.SystemName,
                                       DisplayName = plan.DisplayName,
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

        public async Task<Result<List<PlanFeatureListItemDto>>> GetPublishedPlanFeaturesListByPlanNameAsync(string productName, string planName, CancellationToken cancellationToken = default)
        {
            var planFeatures = await _dbContext.PlanFeatures
                                              .AsNoTracking()
                                              .Where(pf => productName.ToLower().Equals(pf.Plan.Product.SystemName) &&
                                                            planName.ToLower().Equals(pf.Plan.SystemName) && pf.Plan.IsPublished)
                                              .Select(GetPlanFeatureListItemDtoSelector())
                                              .OrderBy(x => x.Plan.DisplayOrder)
                                              .ToListAsync(cancellationToken);

            return Result<List<PlanFeatureListItemDto>>.Successful(planFeatures);
        }

        public Expression<Func<PlanFeature, PlanFeatureListItemDto>> GetPlanFeatureListItemDtoSelector()
        {
            return planFeature => new PlanFeatureListItemDto
            {
                Id = planFeature.Id,
                Reset = planFeature.FeatureReset,
                Limit = planFeature.Limit,
                Unit = planFeature.FeatureUnit,
                UnitDisplayName = planFeature.UnitDisplayName,
                Description = planFeature.Description,
                CreatedDate = planFeature.CreationDate,
                EditedDate = planFeature.ModificationDate,
                Plan = new PlanItemDto
                {
                    Id = planFeature.PlanId,
                    SystemName = planFeature.Plan.SystemName,
                    DisplayName = planFeature.Plan.DisplayName,
                    DisplayOrder = planFeature.Plan.DisplayOrder,
                    IsPublished = planFeature.Plan.IsPublished,
                    IsSubscribed = planFeature.Plan.IsSubscribed,
                },
                Feature = new FeatureItemDto
                {
                    Id = planFeature.Feature.Id,
                    SystemName = planFeature.Feature.SystemName,
                    DisplayName = planFeature.Feature.DisplayName,
                    Type = planFeature.Feature.Type,
                    IsSubscribed = planFeature.Feature.IsSubscribed,
                    DisplayOrder = planFeature.Feature.DisplayOrder,
                    Reset = planFeature.FeatureReset,
                },
            };
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


            var featureType = await _dbContext.Features
                                            .Where(x => x.Id == model.FeatureId &&
                                                        x.ProductId == productId)
                                            .Select(x => x.Type)
                                            .SingleOrDefaultAsync(cancellationToken);

            var date = DateTime.UtcNow;

            var planFeature = new PlanFeature
            {
                Id = Guid.NewGuid(),
                FeatureId = model.FeatureId,
                PlanId = model.PlanId,
                Limit = model.Limit,
                FeatureReset = featureType == FeatureType.Boolean || !model.Reset.HasValue ?
                                                      FeatureReset.NonResettable : model.Reset.Value,
                FeatureUnit = model.Unit,
                UnitDisplayName = model.UnitDisplayName,
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

            var featureType = await _dbContext.Features
                                            .Where(x => x.Id == planFeature.FeatureId &&
                                                        x.ProductId == productId)
                                            .Select(x => x.Type)
                                            .SingleOrDefaultAsync(cancellationToken);

            planFeature.FeatureReset = featureType == FeatureType.Boolean || !model.Reset.HasValue ?
                                                      FeatureReset.NonResettable : model.Reset.Value;
            planFeature.Limit = model.Limit;
            planFeature.FeatureUnit = model.Unit;
            planFeature.UnitDisplayName = model.UnitDisplayName;
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
