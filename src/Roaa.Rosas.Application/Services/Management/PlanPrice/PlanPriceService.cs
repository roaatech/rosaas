using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Application.Services.Management.PlanPrices.Validators;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices
{
    public class PlanPriceService : IPlanPriceService
    {
        #region Props 
        private readonly ILogger<PlanPriceService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public PlanPriceService(
            ILogger<PlanPriceService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services  
        public async Task<Result<List<PlanPriceListItemDto>>> GetPlanPricesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var planPrice = await _dbContext.PlanPrices
                                              .AsNoTracking()
                                              .Where(f => f.Plan.ProductId == productId)
                                              .Select(planPrice => new PlanPriceListItemDto
                                              {
                                                  Id = planPrice.Id,
                                                  Plan = new PlanListItemDto(planPrice.Plan.Id, planPrice.Plan.SystemName, planPrice.Plan.DisplayName, planPrice.Plan.TenancyType, planPrice.Plan.IsLockedBySystem),
                                                  Cycle = planPrice.PlanCycle,
                                                  Price = planPrice.Price,
                                                  IsSubscribed = planPrice.IsSubscribed,
                                                  IsPublished = planPrice.IsPublished,
                                                  IsLockedBySystem = planPrice.IsLockedBySystem,
                                                  SystemName = planPrice.SystemName,
                                                  Description = planPrice.Description,
                                                  CreatedDate = planPrice.CreationDate,
                                                  EditedDate = planPrice.ModificationDate,
                                              })
                                              .OrderByDescending(x => x.EditedDate)
                                              .ToListAsync(cancellationToken);

            return Result<List<PlanPriceListItemDto>>.Successful(planPrice);
        }

        public async Task<Result<List<PlanPricePublishedListItemDto>>> GetPublishedPlanPricesListByProductNameAsync(string productOwnerName, string productName, CancellationToken cancellationToken = default)
        {
            var productId = await _dbContext.Products
                                           .AsNoTracking()
                                           .Include(p => p.ProductOwner)
                                           .Where(p => p.ProductOwner.SystemName == productOwnerName.ToLower() &&
                                                       p.SystemName == productName.ToLower())
                                          .Select(x => x.Id)
                                          .SingleOrDefaultAsync(cancellationToken);

            if (productId == Guid.Empty)
            {
                return Result<List<PlanPricePublishedListItemDto>>.Fail("Product not found");
            }

            var planPrices = await _dbContext.PlanPrices
                                              .AsNoTracking()
                                              .Include(pp => pp.Plan)
                                              .Where(pp => pp.Plan.ProductId == productId && pp.IsPublished && pp.Plan.IsPublished)
                                              .Select(planPrice => new PlanPricePublishedListItemDto
                                              {
                                                  Id = planPrice.Id,
                                                  Plan = new PlanListItemDto { Id = planPrice.PlanId },
                                                  //Product = new LookupItemDto<Guid>(planPrice.Plan.ProductId, product.DisplayName),@Ahmad
                                                  //Client = new LookupItemDto<Guid>(product.ClientId, product.ProductOwner.SystemName),@Ahmad
                                                  Cycle = planPrice.PlanCycle,
                                                  Price = planPrice.Price,
                                                  IsSubscribed = planPrice.IsSubscribed,
                                                  IsPublished = planPrice.IsPublished,
                                                  IsLockedBySystem = planPrice.IsLockedBySystem,
                                                  SystemName = planPrice.SystemName,
                                                  Description = planPrice.Description,
                                                  CreatedDate = planPrice.CreationDate,
                                                  EditedDate = planPrice.ModificationDate,
                                              })
                                              .ToListAsync(cancellationToken);

            return Result<List<PlanPricePublishedListItemDto>>.Successful(planPrices);
        }




        public async Task<Result<PlanPricePublishedDto>> GetPublishedPlanPriceByPlanPriceNameAsync(string productOwnerName, string productName, string planPriceName, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products
                                          .AsNoTracking()
                                          .Where(p => p.ProductOwner!.SystemName.ToLower().Equals(productOwnerName.ToLower()) &&
                                                      p.SystemName.ToLower().Equals(productName.ToLower()))
                                          .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return Result<PlanPricePublishedDto>.Fail("Product not found");
            }

            return await GetPublishedPlanPriceAsync(
                pp => pp.Plan.ProductId == product.Id &&
                      pp.SystemName.ToLower().Equals(planPriceName.ToLower()) &&
                      pp.IsPublished,
                cancellationToken
            );
        }
        public async Task<Result<PlanPricePublishedDto>> GetPublishedPlanPriceByIdAsync(Guid planPriceId, CancellationToken cancellationToken = default)
        {
            return await GetPublishedPlanPriceAsync((pp => pp.Id == planPriceId &&
                                                           pp.IsPublished),
                                                           cancellationToken);
        }

        public async Task<Result<PlanPricePublishedDto>> GetPublishedPlanPriceAsync(Expression<Func<PlanPrice, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var planPrice = await _dbContext.PlanPrices
                                              .AsNoTracking()
                                              .Where(predicate)
                                              .Select(planPrice => new PlanPricePublishedDto
                                              {
                                                  Id = planPrice.Id,
                                                  Plan = new PlanListItemDto(planPrice.Plan.Id, planPrice.Plan.SystemName, planPrice.Plan.DisplayName, planPrice.Plan.TenancyType, planPrice.Plan.IsLockedBySystem),
                                                  Product = new ProductListItemDto(planPrice.Plan.Product.Id, planPrice.Plan.Product.SystemName,
                                                                                    planPrice.Plan.Product.DisplayName, planPrice.Plan.Product.TrialType, planPrice.Plan.Product.TrialPlanId),
                                                  Cycle = planPrice.PlanCycle,
                                                  Price = planPrice.Price,
                                                  IsSubscribed = planPrice.IsSubscribed,
                                                  IsPublished = planPrice.IsPublished,
                                                  IsLockedBySystem = planPrice.IsLockedBySystem,
                                                  SystemName = planPrice.SystemName,
                                                  Description = planPrice.Description,
                                                  CreatedDate = planPrice.CreationDate,
                                                  EditedDate = planPrice.ModificationDate,
                                              })
                                              .SingleOrDefaultAsync(cancellationToken);

            return Result<PlanPricePublishedDto>.Successful(planPrice);
        }

        public async Task<Result<CreatedResult<Guid>>> CreatePlanPriceAsync(CreatePlanPriceModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreatePlanPriceValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            if (!await EnsureUniqueNameAsync(productId, model.SystemName))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(model.SystemName));
            }

            var plan = await _dbContext.Plans.Where(x => x.Id == model.PlanId && x.ProductId == productId).SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(model.PlanId));
            }

            if (plan.TenancyType == Domain.Enums.TenancyType.Planed && (model.Cycle == PlanCycle.Custom || model.Cycle == PlanCycle.Unlimited))
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.PlannedPlanCannotBeCustomized, _identityContextService.Locale, nameof(model.PlanId));
            }

            if (plan.TenancyType == Domain.Enums.TenancyType.Unlimited &&
                plan.TenancyType == Domain.Enums.TenancyType.Limited &&
                model.Price > 0)
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.LimitedAndUnlimitedPlansHaveToBeFree, _identityContextService.Locale, nameof(model.PlanId));
            }

            if (plan.TenancyType == Domain.Enums.TenancyType.Unlimited && model.Cycle != PlanCycle.Unlimited)
            {
                return Result<CreatedResult<Guid>>.Fail(ErrorMessage.UnlimitedPlansHaveToBeUnlimitedCycle, _identityContextService.Locale, nameof(model.PlanId));
            }
            #endregion

            var date = DateTime.UtcNow;

            var planPrice = new PlanPrice
            {
                Id = Guid.NewGuid(),
                SystemName = model.SystemName,
                PlanId = model.PlanId,
                PlanCycle = model.Cycle,
                Price = model.Price,
                Description = model.Description,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.PlanPrices.Add(planPrice);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(planPrice.Id));
        }

        public async Task<Result> UpdatePlanPriceAsync(Guid planPriceId, UpdatePlanPriceModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdatePlanPriceValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            var planPrice = await _dbContext.PlanPrices.Where(x => x.Id == planPriceId).SingleOrDefaultAsync();
            if (planPrice is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }


            if (planPrice.IsLockedBySystem)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueLockedBySystem, _identityContextService.Locale);
            }

            if (planPrice.IsSubscribed)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale);
            }
            #endregion
            PlanPrice featureBeforeUpdate = planPrice.DeepCopy();

            planPrice.PlanCycle = model.Cycle;
            planPrice.Price = model.Price;
            planPrice.Description = model.Description;
            planPrice.ModifiedByUserId = _identityContextService.UserId;
            planPrice.ModificationDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }
        public async Task<Result> PublishPlanPriceAsync(Guid planPriceId, PublishPlanPriceModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 

            var planPrice = await _dbContext.PlanPrices.Where(x => x.Id == planPriceId).SingleOrDefaultAsync();
            if (planPrice is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (planPrice.IsLockedBySystem)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueLockedBySystem, _identityContextService.Locale);
            }

            #endregion

            planPrice.IsPublished = model.IsPublished;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeletePlanPriceAsync(Guid planPriceId, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 
            var planPrice = await _dbContext.PlanPrices.Where(x => x.Id == planPriceId).SingleOrDefaultAsync();
            if (planPrice is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (planPrice.IsLockedBySystem)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueLockedBySystem, _identityContextService.Locale);
            }

            if (planPrice.IsSubscribed)
            {
                return Result.Fail(ErrorMessage.ModificationOrIsNotAllowedDueToSubscription, _identityContextService.Locale);
            }
            #endregion

            _dbContext.PlanPrices.Remove(planPrice);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        #endregion


        private async Task<bool> EnsureUniqueNameAsync(Guid productId, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
        {
            return !await _dbContext.PlanPrices
                                    .Where(x => x.Id != id &&
                                               x.Plan.ProductId == productId &&
                                                uniqueName.ToLower().Equals(x.SystemName))
                                    .AnyAsync(cancellationToken);
        }
    }
}
