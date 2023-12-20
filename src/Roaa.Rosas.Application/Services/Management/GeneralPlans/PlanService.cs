using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.GeneralPlans.Models;
using Roaa.Rosas.Application.Services.Management.GeneralPlans.Validators;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.GeneralPlans
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
                                              Name = plan.SystemName,
                                              Description = plan.Description,
                                              DisplayOrder = plan.DisplayOrder,
                                              CreatedDate = plan.CreationDate,
                                              EditedDate = plan.ModificationDate,
                                          });

            sort = sort.HandleDefaultSorting(new string[] { "Description", "Name", "EditedDate", "CreatedDate" }, "EditedDate", SortDirection.Desc);

            query = query.Where(filters, new string[] { "_Description", "_Name" }, "CreatedDate");

            query = query.OrderBy(sort);

            var pagedUsers = await query.ToPagedResultAsync(paginationInfo, cancellationToken);

            return pagedUsers;
        }

        public async Task<Result<PlanDto>> GetPlanByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var plan = await _dbContext.Plans
                                          .AsNoTracking()
                                          .Where(x => x.Id == id)
                                          .Select(plan => new PlanDto
                                          {
                                              Id = plan.Id,
                                              Name = plan.SystemName,
                                              Description = plan.Description,
                                              DisplayOrder = plan.DisplayOrder,
                                              CreatedDate = plan.CreationDate,
                                              EditedDate = plan.ModificationDate,
                                          })
                                          .SingleOrDefaultAsync(cancellationToken);

            return Result<PlanDto>.Successful(plan);
        }

        public async Task<Result<CreatedResult<Guid>>> CreatePlanAsync(CreatePlanModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new CreatePlanValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result<CreatedResult<Guid>>.New().WithErrors(fValidation.Errors);
            }

            #endregion


            var date = DateTime.UtcNow;

            var id = Guid.NewGuid();
            var plan = new Plan
            {
                Id = id,
                SystemName = model.Name,
                Description = model.Description,
                DisplayOrder = model.DisplayOrder,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.Plans.Add(plan);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(plan.Id));
        }

        public async Task<Result> UpdatePlanAsync(Guid id, UpdatePlanModel model, CancellationToken cancellationToken = default)
        {
            #region Validation
            var fValidation = new UpdatePlanValidator(_identityContextService).Validate(model);
            if (!fValidation.IsValid)
            {
                return Result.New().WithErrors(fValidation.Errors);
            }

            var plan = await _dbContext.Plans.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            #endregion

            Plan planBeforeUpdate = plan.DeepCopy();

            plan.SystemName = model.Name;
            plan.Description = model.Description;
            plan.DisplayOrder = model.DisplayOrder;
            plan.ModifiedByUserId = _identityContextService.UserId;
            plan.ModificationDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }



        public async Task<Result> DeletePlanAsync(Guid id, CancellationToken cancellationToken = default)
        {
            #region Validation 
            var plan = await _dbContext.Plans.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (plan is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }
            #endregion

            _dbContext.Plans.Remove(plan);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }
        #endregion
    }
}
