﻿using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Plans.Models;
using Roaa.Rosas.Application.Services.Management.Plans.Validators;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Plans
{
    public class PlanService : IPlanService
    {
        #region Props 
        private readonly ILogger<PlanService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public PlanService(
            ILogger<PlanService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services   



        public async Task<PaginatedResult<PlanListItemDto>> GetPlansPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Plans.AsNoTracking()
                                          .Select(plan => new PlanListItemDto
                                          {
                                              Id = plan.Id,
                                              SystemName = plan.SystemName,
                                              DisplayName = plan.DisplayName,
                                              Description = plan.Description,
                                              DisplayOrder = plan.DisplayOrder,
                                              CreatedDate = plan.CreationDate,
                                              EditedDate = plan.ModificationDate,
                                              Product = new LookupItemDto<Guid>(plan.ProductId, plan.Product.DisplayName),
                                              IsPublished = plan.IsPublished,
                                              IsSubscribed = plan.IsSubscribed,
                                              IsLockedBySystem = plan.IsLockedBySystem,
                                              TenancyType = plan.TenancyType,
                                              TrialPeriodInDays = plan.TrialPeriodInDays,
                                              AlternativePlanId = plan.AlternativePlanId,
                                              AlternativePlanPriceId = plan.AlternativePlanPriceId,
                                          });

            sort = sort.HandleDefaultSorting(new string[] { "Description", "SystemName", "DisplayName", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(filters, new string[] { "ProductId", "_Description", "_SystemName", "_DisplayName" }, "CreatedDate");

            query = query.OrderBy(sort);

            var pagedUsers = await query.ToPagedResultAsync(paginationInfo, cancellationToken);

            return pagedUsers;
        }
        public async Task<Result<List<PlanListItemDto>>> GetPlansListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var plans = await _dbContext.Plans
                                              .AsNoTracking()
                                              .Where(f => f.ProductId == productId)
                                              .Select(plan => new PlanListItemDto
                                              {
                                                  Id = plan.Id,
                                                  SystemName = plan.SystemName,
                                                  DisplayName = plan.DisplayName,
                                                  Description = plan.Description,
                                                  DisplayOrder = plan.DisplayOrder,
                                                  CreatedDate = plan.CreationDate,
                                                  EditedDate = plan.ModificationDate,
                                                  Product = new LookupItemDto<Guid>(plan.ProductId, plan.Product.DisplayName),
                                                  IsPublished = plan.IsPublished,
                                                  IsSubscribed = plan.IsSubscribed,
                                                  IsLockedBySystem = plan.IsLockedBySystem,
                                                  TenancyType = plan.TenancyType,
                                                  TrialPeriodInDays = plan.TrialPeriodInDays,
                                                  AlternativePlanId = plan.AlternativePlanId,
                                                  AlternativePlanPriceId = plan.AlternativePlanPriceId,
                                              })
                                              .OrderByDescending(x => x.EditedDate)
                                              .ToListAsync(cancellationToken);

            return Result<List<PlanListItemDto>>.Successful(plans);
        }

        public async Task<Result<List<PlanPublishedListItemDto>>> GetPublishedPlansListByProductNameAsync(string productName, CancellationToken cancellationToken = default)
        {
            var plans = await _dbContext.Plans
                                              .AsNoTracking()
                                              .Where(pp => productName.ToLower().Equals(pp.Product.SystemName))
                                              .Select(plan => new PlanPublishedListItemDto
                                              {
                                                  Id = plan.Id,
                                                  SystemName = plan.SystemName,
                                                  DisplayName = plan.DisplayName,
                                                  Description = plan.Description,
                                                  DisplayOrder = plan.DisplayOrder,
                                                  CreatedDate = plan.CreationDate,
                                                  EditedDate = plan.ModificationDate,
                                                  Product = new LookupItemDto<Guid>(plan.ProductId, plan.Product.DisplayName),
                                                  Client = new LookupItemDto<Guid>(plan.Product.ClientId, plan.Product.Client.SystemName),
                                                  IsPublished = plan.IsPublished,
                                                  IsSubscribed = plan.IsSubscribed,
                                                  IsLockedBySystem = plan.IsLockedBySystem,
                                                  TenancyType = plan.TenancyType,
                                                  TrialPeriodInDays = plan.TrialPeriodInDays,
                                                  AlternativePlanId = plan.AlternativePlanId,
                                                  AlternativePlanPriceId = plan.AlternativePlanPriceId,
                                              })
                                              .OrderByDescending(x => x.DisplayOrder)
                                              .ToListAsync(cancellationToken);

            return Result<List<PlanPublishedListItemDto>>.Successful(plans);
        }

        public async Task<Result<List<ExternalSystemPlanListItemDto>>> GetPlansListOfExternalSystemByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var plans = await _dbContext.Plans
                                              .AsNoTracking()
                                              .Where(f => f.ProductId == productId)
                                              .Select(plan => new ExternalSystemPlanListItemDto
                                              {
                                                  SystemName = plan.SystemName,
                                                  DisplayName = plan.DisplayName,
                                                  Description = plan.Description,
                                                  DisplayOrder = plan.DisplayOrder,
                                                  IsPublished = plan.IsPublished,
                                                  IsSubscribed = plan.IsSubscribed,
                                                  IsLockedBySystem = plan.IsLockedBySystem,
                                                  TenancyType = plan.TenancyType,
                                                  TenancyTypeName = plan.TenancyType.ToString(),
                                                  Prices = plan.Prices.Select(planPrice => new ExternalSystemPlanPriceListItemDto
                                                  {
                                                      SystemName = planPrice.SystemName,
                                                      Description = planPrice.Description,
                                                      Cycle = planPrice.PlanCycle,
                                                      CycleName = planPrice.PlanCycle.ToString(),
                                                      Price = planPrice.Price,
                                                      IsSubscribed = planPrice.IsSubscribed,
                                                      IsPublished = planPrice.IsPublished,
                                                      IsLockedBySystem = planPrice.IsLockedBySystem,

                                                  }).ToList(),
                                              })
                                              .OrderByDescending(x => x.DisplayOrder)
                                              .ToListAsync(cancellationToken);

            return Result<List<ExternalSystemPlanListItemDto>>.Successful(plans);
        }



        public async Task<Result<List<LookupItemDto<Guid>>>> GetPlansLookupListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var plans = await _dbContext.Plans
                                              .AsNoTracking()
                                              .Where(f => f.ProductId == productId)
                                              .Select(plan => new LookupItemDto<Guid>
                                              {
                                                  Id = plan.Id,
                                                  SystemName = plan.DisplayName,
                                              })
                                               .OrderBy(x => x.SystemName)
                                              .ToListAsync(cancellationToken);

            return Result<List<LookupItemDto<Guid>>>.Successful(plans);
        }


        public async Task<Result<PlanDto>> GetPlanByIdAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            var plan = await _dbContext.Plans
                                          .AsNoTracking()
                                          .Where(x => x.Id == id && x.ProductId == productId)
                                          .Select(plan => new PlanDto
                                          {
                                              Id = plan.Id,
                                              SystemName = plan.SystemName,
                                              DisplayName = plan.DisplayName,
                                              Description = plan.Description,
                                              DisplayOrder = plan.DisplayOrder,
                                              CreatedDate = plan.CreationDate,
                                              EditedDate = plan.ModificationDate,
                                              Product = new LookupItemDto<Guid>(plan.ProductId, plan.Product.DisplayName),
                                              IsPublished = plan.IsPublished,
                                              IsSubscribed = plan.IsSubscribed,
                                              IsLockedBySystem = plan.IsLockedBySystem,
                                              TenancyType = plan.TenancyType,
                                              TrialPeriodInDays = plan.TrialPeriodInDays,
                                              AlternativePlanId = plan.AlternativePlanId,
                                              AlternativePlanPriceId = plan.AlternativePlanPriceId,
                                          })
                                          .SingleOrDefaultAsync(cancellationToken);

            return Result<PlanDto>.Successful(plan);
        }

        public async Task<Result<CreatedResult<Guid>>> CreatePlanAsync(CreatePlanModel model,
                                                                        Guid productId,
                                                                        CancellationToken cancellationToken = default,
                                                                        TenancyType tenancyType = TenancyType.Planed,
                                                                        bool isLockedBySystem = false)
        {
            #region Validation
            var fValidation = new CreatePlanValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            if (!await _dbContext.Products.Where(x => x.Id == productId).AnyAsync(cancellationToken))
            {
                return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, "productId");
            }

            if (!await EnsureUniqueNameAsync(productId, model.SystemName))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.SystemName));
            }

            #endregion


            var date = DateTime.UtcNow;

            PlanPrice? alternativePlanPrice = null;
            Guid? alternativePlanId = model.AlternativePlanId;

            if (model.AlternativePlanId is not null)
            {
                var alternativePlanPrices = await _dbContext.PlanPrices
                                                         .Where(x => x.PlanId == model.AlternativePlanId)
                                                         .ToListAsync(cancellationToken);
                alternativePlanPrice = alternativePlanPrices.Where(x => x.Price == 0).Any() ?
                                        alternativePlanPrices.Where(x => x.Price == 0).FirstOrDefault() :
                                        alternativePlanPrices.FirstOrDefault();

                if (alternativePlanPrice is null)
                {
                    return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(model.AlternativePlanId));
                }

                alternativePlanId = alternativePlanPrice.PlanId;
            }

            var id = Guid.NewGuid();
            var plan = new Plan
            {
                Id = id,
                ProductId = model.ProductId,
                SystemName = model.SystemName,
                DisplayName = model.DisplayName,
                Description = model.Description,
                DisplayOrder = model.DisplayOrder,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
                TenancyType = tenancyType,
                IsLockedBySystem = isLockedBySystem,
                TrialPeriodInDays = model.TrialPeriodInDays,
                AlternativePlanId = alternativePlanId,
                AlternativePlanPriceId = alternativePlanPrice?.Id,
            };

            _dbContext.Plans.Add(plan);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(plan.Id));
        }

        public async Task<Result> UpdatePlanAsync(Guid id, UpdatePlanModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdatePlanValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            var plan = await _dbContext.Plans.Where(x => x.Id == id && x.ProductId == productId).SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (plan.IsLockedBySystem)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueLockedBySystem, _identityContextService.Locale);
            }

            if (plan.IsSubscribed)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale);
            }

            if (!await EnsureUniqueNameAsync(productId, model.SystemName, id))
            {
                return Result.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.SystemName));
            }
            #endregion
            PlanPrice? alternativePlanPrice = null;
            Guid? alternativePlanId = model.AlternativePlanId;

            if (model.AlternativePlanId is not null)
            {
                var alternativePlanPrices = await _dbContext.PlanPrices
                                                         .Where(x => x.PlanId == model.AlternativePlanId)
                                                         .ToListAsync(cancellationToken);
                alternativePlanPrice = alternativePlanPrices.Where(x => x.Price == 0).Any() ?
                                        alternativePlanPrices.Where(x => x.Price == 0).FirstOrDefault() :
                                        alternativePlanPrices.FirstOrDefault();

                if (alternativePlanPrice is null)
                {
                    return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(model.AlternativePlanId));
                }

                alternativePlanId = alternativePlanPrice.PlanId;
            }

            //Plan planBeforeUpdate = plan.DeepCopy();

            plan.SystemName = model.SystemName;
            plan.DisplayName = model.DisplayName;
            plan.Description = model.Description;
            plan.DisplayOrder = model.DisplayOrder;
            plan.ModifiedByUserId = _identityContextService.UserId;
            plan.ModificationDate = DateTime.UtcNow;
            plan.TrialPeriodInDays = model.TrialPeriodInDays;
            plan.AlternativePlanId = alternativePlanId;
            plan.AlternativePlanPriceId = alternativePlanPrice?.Id;


            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public async Task<Result> PublishPlanAsync(Guid id, PublishPlanModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 

            var plan = await _dbContext.Plans.Where(x => x.Id == id && x.ProductId == productId).SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (plan.IsLockedBySystem)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueLockedBySystem, _identityContextService.Locale);
            }


            #endregion 

            plan.IsPublished = model.IsPublished;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeletePlanAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 
            var plan = await _dbContext.Plans.Where(x => x.Id == id && x.ProductId == productId).SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (plan.IsLockedBySystem)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueLockedBySystem, _identityContextService.Locale);
            }


            if (plan.IsSubscribed)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale);
            }

            #endregion

            var planFeatures = await _dbContext.PlanFeatures.Where(x => x.PlanId == id).ToListAsync(cancellationToken);
            if (planFeatures.Any())
            {
                _dbContext.PlanFeatures.RemoveRange(planFeatures);
            }

            var planPrices = await _dbContext.PlanPrices.Where(x => x.PlanId == id).ToListAsync(cancellationToken);
            if (planPrices.Any())
            {
                _dbContext.PlanPrices.RemoveRange(planPrices);
            }

            _dbContext.Plans.Remove(plan);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        private async Task<bool> DeletingIsAllowedAsync(Guid planId, CancellationToken cancellationToken = default)
        {
            return !await _dbContext.PlanFeatures
                                    .Where(x => x.PlanId == planId && x.Plan.IsPublished)
                                    .AnyAsync(cancellationToken);

        }
        #endregion


        private async Task<bool> EnsureUniqueNameAsync(Guid productId, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
        {
            return !await _dbContext.Plans
                                    .Where(x => x.Id != id &&
                                               x.ProductId == productId &&
                                                uniqueName.ToLower().Equals(x.SystemName))
                                    .AnyAsync(cancellationToken);
        }
    }
}
