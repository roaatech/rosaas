﻿using FluentValidation;
using Microsoft.AspNetCore.Hosting;
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

namespace Roaa.Rosas.Application.Services.Management.Plans
{
    public class PlanService : IPlanService
    {
        #region Props 
        private readonly ILogger<PlanService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public PlanService(
            ILogger<PlanService> logger,
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



        public async Task<PaginatedResult<PlanListItemDto>> GetPlansPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Plans.AsNoTracking()
                                          .Select(plan => new PlanListItemDto
                                          {
                                              Id = plan.Id,
                                              Name = plan.Name,
                                              Description = plan.Description,
                                              DisplayOrder = plan.DisplayOrder,
                                              CreatedDate = plan.CreationDate,
                                              EditedDate = plan.ModificationDate,
                                              Product = new LookupItemDto<Guid>(plan.ProductId, plan.Product.Name),
                                          });

            sort = sort.HandleDefaultSorting(new string[] { "Description", "Name", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(filters, new string[] { "ProductId", "_Description", "_Name" }, "CreatedDate");

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
                                                  Name = plan.Name,
                                                  Description = plan.Description,
                                                  DisplayOrder = plan.DisplayOrder,
                                                  CreatedDate = plan.CreationDate,
                                                  EditedDate = plan.ModificationDate,
                                                  Product = new LookupItemDto<Guid>(plan.ProductId, plan.Product.Name),
                                              })
                                              .OrderByDescending(x => x.EditedDate)
                                              .ToListAsync(cancellationToken);

            return Result<List<PlanListItemDto>>.Successful(plans);
        }

        public async Task<Result<List<LookupItemDto<Guid>>>> GetPlansLookupListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var plans = await _dbContext.Plans
                                              .AsNoTracking()
                                              .Where(f => f.ProductId == productId)
                                              .Select(plan => new LookupItemDto<Guid>
                                              {
                                                  Id = plan.Id,
                                                  Name = plan.Name,
                                              })
                                               .OrderBy(x => x.Name)
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
                                              Name = plan.Name,
                                              Description = plan.Description,
                                              DisplayOrder = plan.DisplayOrder,
                                              CreatedDate = plan.CreationDate,
                                              EditedDate = plan.ModificationDate,
                                              Product = new LookupItemDto<Guid>(plan.ProductId, plan.Product.Name),
                                          })
                                          .SingleOrDefaultAsync(cancellationToken);

            return Result<PlanDto>.Successful(plan);
        }

        public async Task<Result<CreatedResult<Guid>>> CreatePlanAsync(CreatePlanModel model, Guid productId, CancellationToken cancellationToken = default)
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
            #endregion


            var date = DateTime.UtcNow;

            var id = Guid.NewGuid();
            var plan = new Plan
            {
                Id = id,
                ProductId = model.ProductId,
                Name = model.Name,
                Description = model.Description,
                DisplayOrder = model.DisplayOrder,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
                IsPublished = true,
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

            #endregion
            Plan planBeforeUpdate = plan.DeepCopy();

            plan.Name = model.Name;
            plan.Description = model.Description;
            plan.DisplayOrder = model.DisplayOrder;
            plan.ModifiedByUserId = _identityContextService.UserId;
            plan.ModificationDate = DateTime.UtcNow;

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

            if (!await DeletingIsAllowedAsync(id, cancellationToken))
            {
                return Result.Fail(ErrorMessage.DeletingIsNotAllowed, _identityContextService.Locale);
            }
            #endregion

            _dbContext.Plans.Remove(plan);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

        private async Task<bool> DeletingIsAllowedAsync(Guid planId, CancellationToken cancellationToken = default)
        {
            return !await _dbContext.PlanFeatures
                                    .Where(x => x.PlanId == planId)
                                    .AnyAsync(cancellationToken);

        }
        #endregion
    }
}
