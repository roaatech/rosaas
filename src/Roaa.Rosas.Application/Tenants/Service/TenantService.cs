using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

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
        public async Task<Result<ChangeTenantStatusResult>> ChangeTenantStatusAsync(ChangeTenantStatusModel model, CancellationToken cancellationToken = default)
        {
            var tenant = await _dbContext.Tenants.Where(x => x.Id == model.TenantId).SingleOrDefaultAsync();
            if (tenant is null)
            {
                return Result<ChangeTenantStatusResult>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            var nextProcess = await _workflow.GetNextProcessActionAsync(tenant.Status, model.Status, model.UserType, model.Action);
            if (nextProcess is null)
            {
                return Result<ChangeTenantStatusResult>.Fail(CommonErrorKeys.UnAuthorizedAction, _identityContextService.Locale);
            }

            var statusBeforeUpdate = tenant.Status;

            tenant.Status = nextProcess.NextStatus;
            tenant.EditedByUserId = model.EditorBy;
            tenant.Edited = DateTime.UtcNow;


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

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<ChangeTenantStatusResult>.Successful(new ChangeTenantStatusResult(tenant, nextProcess));
        }
        #endregion
    }
}
