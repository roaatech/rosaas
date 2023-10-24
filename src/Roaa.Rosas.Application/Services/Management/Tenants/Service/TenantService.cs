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
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using System.Linq.Expressions;

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



        public async Task<Result<List<TenantStatusChangedResultDto>>> SetTenantNextStatusAsync(Guid tenantId, TenantStatus status, Guid? productId, string notes, CancellationToken cancellationToken)
        {
            var stepStatus = await _workflow.GetStepStatusAsync(status, cancellationToken);


            var result = await SetTenantNextStatusAsync(new SetTenantNextStatusModel
            {
                UserType = _identityContextService.GetUserType(),
                Status = status,
                Step = null,
                Action = WorkflowAction.Ok,
                TenantId = tenantId,
                ProductId = productId,
                Notes = notes,
                EditorBy = _identityContextService.GetActorId(),
            });

            if (!result.Success)
            {
                return Result<List<TenantStatusChangedResultDto>>.Fail(result.Messages);
            }


            var dtos = (await Task.WhenAll(result.Data.Select(async item =>
            {
                var action = (await _workflow.GetProcessActionsAsync(item.Subscription.Status, item.Subscription.Step, _identityContextService.GetUserType())).ToActionsResults();
                return new TenantStatusChangedResultDto(item.Subscription.ProductId, item.Subscription.Status, action);
            })))
            .Where(result => result != null)
            .ToList();


            return Result<List<TenantStatusChangedResultDto>>.Successful(dtos);
        }



        public async Task<Result<List<SetTenantNextStatusResult>>> SetTenantNextStatusAsync(SetTenantNextStatusModel model, CancellationToken cancellationToken = default)
        {
            #region Validation

            // Preparing to Retrieve Subscriptions 
            Expression<Func<Subscription, bool>> predicate = x => x.TenantId == model.TenantId;
            if (model.ProductId is not null)
            {
                predicate = x => x.TenantId == model.TenantId && x.ProductId == model.ProductId;
            }


            // Subscriptions retrieving  
            var subscriptions = await _dbContext.Subscriptions.Where(predicate).ToListAsync(cancellationToken);


            // Subscriptions validating  
            if (subscriptions is null || !subscriptions.Any())
            {
                return Result<List<SetTenantNextStatusResult>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }


            // Getting the next status of the subscriptions' workflow 

            var workflows = (await Task.WhenAll(subscriptions.Select(async subscription =>
            {
                return await _workflow.GetNextProcessActionAsync(currentStatus: subscription.Status,
                                                                 currentStep: subscription.Step,
                                                                 nextStatus: model.Status,
                                                                 userType: model.UserType,
                                                                 action: model.Action);
            })))
            .Where(result => result != null)
            .ToList();

            if (workflows is null || !workflows.Any())
            {
                return Result<List<SetTenantNextStatusResult>>.Fail(ErrorMessage.NotAllowedChangeStatus, _identityContextService.Locale);
            }
            #endregion 

            List<SetTenantNextStatusResult> results = new();

            DateTime date = DateTime.UtcNow;

            foreach (var subscription in subscriptions)
            {
                var workfolw = workflows.Where(x => x.CurrentStatus == subscription.Status).FirstOrDefault();

                if (workfolw is not null)
                {
                    subscription.Status = workfolw.NextStatus;
                    subscription.Step = workfolw.NextStep;
                    subscription.ModifiedByUserId = model.EditorBy;
                    subscription.ModificationDate = date;
                    subscription.Notes = model.Notes;

                    results.Add(new SetTenantNextStatusResult(subscription, workfolw));

                    subscription.AddDomainEvent(new TenantStatusUpdatedEvent(subscription, workfolw, workfolw.CurrentStatus, workfolw.CurrentStep, model.Notes));
                }

            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<List<SetTenantNextStatusResult>>.Successful(results);
        }

        #endregion
    }
}
