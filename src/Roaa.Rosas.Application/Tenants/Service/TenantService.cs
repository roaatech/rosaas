using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Tenants.Service
{
    public partial class TenantService : ITenantService
    {
        #region Props 
        private readonly ILogger<TenantService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantWorkflow _workflow;
        #endregion


        #region Corts
        public TenantService(
            ILogger<TenantService> logger,
            IRosasDbContext dbContext,
            IWebHostEnvironment environment,
            ITenantWorkflow workflow,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _environment = environment;
            _workflow = workflow;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services    
        public async Task<Result<List<ChangeTenantStatusResult>>> ChangeTenantStatusAsync(ChangeTenantStatusModel model, CancellationToken cancellationToken = default)
        {
            Expression<Func<ProductTenant, bool>> predicate = x => x.TenantId == model.TenantId;
            if (model.ProductId is not null)
            {
                predicate = x => x.TenantId == model.TenantId && x.ProductId == model.ProductId;
            }

            var productTenants = await _dbContext.ProductTenants.Where(predicate).ToListAsync(cancellationToken);
            if (productTenants is null || !productTenants.Any())
            {
                return Result<List<ChangeTenantStatusResult>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }


            var nextProcesses = await _workflow.GetNextProcessActionsAsync(productTenants.Select(x => x.Status).ToList(), model.Status, model.UserType, model.Action);
            if (nextProcesses is null || !nextProcesses.Any())
            {
                return Result<List<ChangeTenantStatusResult>>.Fail(CommonErrorKeys.UnAuthorizedAction, _identityContextService.Locale);
            }


            List<ChangeTenantStatusResult> results = new();

            foreach (var tenant in productTenants)
            {
                var nextProcess = nextProcesses.Where(x => x.CurrentStatus == tenant.Status).FirstOrDefault();
                if (nextProcess is not null)
                {
                    tenant.Status = nextProcess.NextStatus;
                    tenant.EditedByUserId = model.EditorBy;
                    tenant.Edited = DateTime.UtcNow;

                    if (nextProcess.CurrentStatus == Domain.Enums.TenantStatus.Active)
                    {
                        tenant.AddDomainEvent(new ActiveTenantStatusUpdated(tenant));
                    }

                    var process = new TenantProcess
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenant.Id,
                        Status = nextProcess.NextStatus,
                        PreviousStatus = nextProcess.CurrentStatus,
                        OwnerId = _identityContextService.GetActorId(),
                        OwnerType = _identityContextService.GetUserType(),
                        Created = DateTime.UtcNow,
                        Message = nextProcess.Message
                    };

                    _dbContext.TenantProcesses.Add(process);


                    results.Add(new ChangeTenantStatusResult(tenant, nextProcess));
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<List<ChangeTenantStatusResult>>.Successful(results);
        }
        #endregion
    }
}
