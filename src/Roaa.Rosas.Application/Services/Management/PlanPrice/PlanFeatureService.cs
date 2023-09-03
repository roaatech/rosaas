using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Application.Services.Management.PlanPrices.Validators;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices
{
    public class PlanPriceService : IPlanPriceService
    {
        #region Props 
        private readonly ILogger<PlanPriceService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public PlanPriceService(
            ILogger<PlanPriceService> logger,
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
        public async Task<Result<List<PlanPriceListItemDto>>> GetPlanPricesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var planPrice = await _dbContext.PlanPrices
                                              .AsNoTracking()
                                              .Where(f => f.Plan.ProductId == productId)
                                              .Select(planPrice => new PlanPriceListItemDto
                                              {
                                                  Id = planPrice.Id,
                                                  Plan = new LookupItemDto<Guid>(planPrice.Plan.Id, planPrice.Plan.Name),
                                                  Cycle = planPrice.Cycle,
                                                  Price = planPrice.Price,
                                                  Description = planPrice.Description,
                                                  CreatedDate = planPrice.Created,
                                                  EditedDate = planPrice.Edited,
                                              })
                                              .OrderByDescending(x => x.EditedDate)
                                              .ToListAsync(cancellationToken);

            return Result<List<PlanPriceListItemDto>>.Successful(planPrice);
        }

        public async Task<Result<CreatedResult<Guid>>> CreatePlanPriceAsync(CreatePlanPriceModel model, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreatePlanPriceValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            #endregion

            var date = DateTime.UtcNow;

            var planPrice = new PlanPrice
            {
                Id = Guid.NewGuid(),
                PlanId = model.PlanId,
                Cycle = model.Cycle,
                Price = model.Price,
                Description = model.Description,
                CreatedByUserId = _identityContextService.UserId,
                EditedByUserId = _identityContextService.UserId,
                Created = date,
                Edited = date,
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

            if (!await UpdatingOrDeletingIsAllowedAsync(planPriceId, cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion
            PlanPrice featureBeforeUpdate = planPrice.DeepCopy();

            planPrice.Cycle = model.Cycle;
            planPrice.Price = model.Price;
            planPrice.Description = model.Description;
            planPrice.EditedByUserId = _identityContextService.UserId;
            planPrice.Edited = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeletePlanPriceAsync(Guid planPriceId, Guid productId, CancellationToken cancellationToken = default)
        {
            #region Validation 
            var feature = await _dbContext.PlanPrices.Where(x => x.Id == planPriceId).SingleOrDefaultAsync();
            if (feature is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (!await UpdatingOrDeletingIsAllowedAsync(planPriceId, cancellationToken))
            {
                return Result.Fail(CommonErrorKeys.OperationIsNotAllowed, _identityContextService.Locale);
            }
            #endregion

            _dbContext.PlanPrices.Remove(feature);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        private async Task<bool> UpdatingOrDeletingIsAllowedAsync(Guid planPriceId, CancellationToken cancellationToken = default)
        {
            return true;
        }
        #endregion
    }
}
