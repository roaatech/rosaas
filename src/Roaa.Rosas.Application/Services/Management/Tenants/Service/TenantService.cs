using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using System.Linq.Expressions;
using static Roaa.Rosas.Domain.Entities.Management.TenantProcessData;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service
{
    public partial class TenantService : ITenantService
    {
        #region Props 
        private readonly ILogger<TenantService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantWorkflow _workflow;
        private readonly IPublisher _publisher;
        private readonly BackgroundServicesStore _backgroundServicesStore;
        #endregion


        #region Corts
        public TenantService(
            ILogger<TenantService> logger,
            IRosasDbContext dbContext,
            IWebHostEnvironment environment,
            ITenantWorkflow workflow,
            IPublisher publisher,
            IIdentityContextService identityContextService,
            BackgroundServicesStore backgroundServicesStore)
        {
            _logger = logger;
            _dbContext = dbContext;
            _environment = environment;
            _workflow = workflow;
            _publisher = publisher;
            _identityContextService = identityContextService;
            _backgroundServicesStore = backgroundServicesStore;
        }

        #endregion


        #region Services   
        public async Task<Result<T>> GetByIdAsync<T>(Guid tenantId, Expression<Func<Tenant, T>> selector, CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.Tenants
                                         .Where(x => x.Id == tenantId)
                                         .Select(selector)
                                         .SingleOrDefaultAsync(cancellationToken);

            return Result<T>.Successful(result);
        }

        public async Task<Result<List<TenantStatusChangedResultDto>>> ChangeTenantStatusAsync(ChangeTenantStatusModel model, CancellationToken cancellationToken)
        {
            // #1 - Change Status   
            var result = await SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                UserType = _identityContextService.GetUserType(),
                Status = model.Status,
                Action = WorkflowAction.Ok,
                TenantId = model.TenantId,
                ProductId = model.ProductId,
                Notes = model.Notes,
                EditorBy = _identityContextService.GetActorId(),
            });

            if (!result.Success)
            {
                return Result<List<TenantStatusChangedResultDto>>.Fail(result.Messages);
            }


            // #2 - Publish Events by status (Call External Systems)
            foreach (var resultItem in result.Data)
            {
                var statusManager = TenantStatusManager.FromKey(resultItem.ProductTenant.Status);

                await statusManager.PublishEventAsync(_publisher, resultItem.ProductTenant, resultItem.Process.CurrentStatus, cancellationToken);
            }

            // #3 - Retrieve The Results (Updated Status & Process Actions)
            Expression<Func<Subscription, bool>> predicate = x => x.TenantId == model.TenantId;
            if (model.ProductId is not null)
            {
                predicate = x => x.TenantId == model.TenantId && x.ProductId == model.ProductId;
            }

            var updatedStatuses = await _dbContext.Subscriptions
                                                .Where(predicate)
                                                .Select(x => new { x.Status, x.ProductId })
                                                .ToListAsync(cancellationToken);

            List<TenantStatusChangedResultDto> results = new();
            foreach (var item in updatedStatuses)
            {
                results.Add(new TenantStatusChangedResultDto(
                item.ProductId,
                item.Status,
                (await _workflow.GetProcessActionsAsync(item.Status,
                                                        _identityContextService.GetUserType()))
                                .ToActionsResults()));
            }

            return Result<List<TenantStatusChangedResultDto>>.Successful(results);
        }

        public async Task<Result<List<SetTenantNextStatusResult>>> SetTenantNextStatusAsync(SetTenantNextStatusModel model, CancellationToken cancellationToken = default)
        {
            Expression<Func<Subscription, bool>> predicate = x => x.TenantId == model.TenantId;
            if (model.ProductId is not null)
            {
                predicate = x => x.TenantId == model.TenantId && x.ProductId == model.ProductId;
            }

            var subscriptions = await _dbContext.Subscriptions.Where(predicate).ToListAsync(cancellationToken);
            if (subscriptions is null || !subscriptions.Any())
            {
                return Result<List<SetTenantNextStatusResult>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }


            var nextProcesses = await _workflow.GetNextProcessActionsAsync(subscriptions.Select(x => x.Status).ToList(), model.Status, model.UserType, model.Action);
            if (nextProcesses is null || !nextProcesses.Any())
            {
                return Result<List<SetTenantNextStatusResult>>.Fail(CommonErrorKeys.UnAuthorizedAction, _identityContextService.Locale);
            }


            List<SetTenantNextStatusResult> results = new();

            DateTime date = DateTime.UtcNow;

            foreach (var subscription in subscriptions)
            {
                var nextProcess = nextProcesses.Where(x => x.CurrentStatus == subscription.Status).FirstOrDefault();
                if (nextProcess is not null)
                {
                    subscription.Status = nextProcess.NextStatus;
                    subscription.ModifiedByUserId = model.EditorBy;
                    subscription.ModificationDate = date;
                    subscription.Notes = model.Notes;

                    if (nextProcess.CurrentStatus == Domain.Enums.TenantStatus.Active)
                    {
                        subscription.AddDomainEvent(new ActiveTenantStatusUpdated(subscription));
                    }

                    var statusHistory = new TenantStatusHistory
                    {
                        Id = Guid.NewGuid(),
                        TenantId = subscription.TenantId,
                        ProductId = subscription.ProductId,
                        SubscriptionId = subscription.Id,
                        Status = nextProcess.NextStatus,
                        PreviousStatus = nextProcess.CurrentStatus,
                        OwnerId = _identityContextService.GetActorId(),
                        OwnerType = _identityContextService.GetUserType(),
                        Created = date,
                        TimeStamp = date,
                        Message = nextProcess.Message,
                    };

                    _dbContext.TenantStatusHistory.Add(statusHistory);

                    var processData = new TenantStatusChangedProcessData
                    {
                        PreviousStatus = nextProcess.CurrentStatus,
                        Status = nextProcess.NextStatus,
                    };

                    var processHistory = new TenantProcessHistory
                    {
                        Id = Guid.NewGuid(),
                        TenantId = subscription.TenantId,
                        ProductId = subscription.ProductId,
                        SubscriptionId = subscription.Id,
                        Status = nextProcess.NextStatus,
                        OwnerId = _identityContextService.GetActorId(),
                        OwnerType = _identityContextService.GetUserType(),
                        ProcessDate = date,
                        TimeStamp = date,
                        ProcessType = TenantProcessType.StatusChanged,
                        Enabled = true,
                        Data = System.Text.Json.JsonSerializer.Serialize(processData),
                        Notes = model.Notes,
                    };

                    _dbContext.TenantProcessHistory.Add(processHistory);

                    results.Add(new SetTenantNextStatusResult(subscription, nextProcess));
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            foreach (var tenantProduct in subscriptions)
            {
                _backgroundServicesStore.RemoveTenantProcess(tenantProduct.TenantId, tenantProduct.ProductId);
            }

            return Result<List<SetTenantNextStatusResult>>.Successful(results);
        }

        #endregion
    }
}
