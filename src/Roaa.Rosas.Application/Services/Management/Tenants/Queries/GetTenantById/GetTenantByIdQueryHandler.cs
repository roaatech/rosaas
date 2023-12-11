using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantById
{
    public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, Result<TenantDto>>
    {
        #region Props 
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantWorkflow _workflow;
        private readonly IEntityAdminPrivilegeService _entityAdminPrivilegeService;
        #endregion


        #region Corts
        public GetTenantByIdQueryHandler(
                            IRosasDbContext dbContext,
                            ITenantWorkflow workflow,
                            IEntityAdminPrivilegeService entityAdminPrivilegeService,
                            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _workflow = workflow;
            _entityAdminPrivilegeService = entityAdminPrivilegeService;
        }

        #endregion


        #region Handler   
        public async Task<Result<TenantDto>> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {

            var tenant = await _dbContext.Tenants
                                        .AsNoTracking()
                                        .Where(x => _identityContextService.IsSuperAdmin() ||
                                                    _dbContext.EntityAdminPrivileges
                                                                .Any(a =>
                                                                    a.UserId == _identityContextService.UserId &&
                                                                    a.EntityId == x.Id &&
                                                                    a.EntityType == EntityType.Tenant
                                                                    )
                                                )
                                            .Where(x => x.Id == request.Id)
                                            .Select(tenant => new TenantDto
                                            {
                                                Id = tenant.Id,
                                                UniqueName = tenant.UniqueName,
                                                Title = tenant.DisplayName,
                                                CreatedDate = tenant.CreationDate,
                                                EditedDate = tenant.ModificationDate,
                                                Subscriptions = tenant.Subscriptions.Select(subscription => new SubscriptionDto
                                                {
                                                    Product = new LookupItemDto<Guid>
                                                    {
                                                        Id = subscription.Product.Id,
                                                        Name = subscription.Product.DisplayName,
                                                    },
                                                    Plan = new LookupItemDto<Guid>
                                                    {
                                                        Id = subscription.Plan.Id,
                                                        Name = subscription.Plan.DisplayName,
                                                    },
                                                    ProductId = subscription.ProductId,
                                                    SubscriptionId = subscription.Id,
                                                    ProductName = subscription.Product.DisplayName,
                                                    Status = subscription.Status,
                                                    Step = subscription.Step,
                                                    ExpectedResourceStatus = subscription.ExpectedResourceStatus,
                                                    CreatedDate = subscription.CreationDate,
                                                    EditedDate = subscription.ModificationDate,
                                                    StartDate = subscription.StartDate,
                                                    EndDate = subscription.EndDate,
                                                    Metadata = subscription.Metadata,
                                                    HealthCheckUrl = subscription.HealthCheckUrl,
                                                    HealthCheckUrlIsOverridden = subscription.HealthCheckUrlIsOverridden,
                                                    HealthCheckStatus = new ProductTenantHealthStatusDto
                                                    {
                                                        HealthCheckUrl = subscription.HealthCheckStatus.HealthCheckUrl,
                                                        IsHealthy = subscription.HealthCheckStatus.IsHealthy,
                                                        LastCheckDate = subscription.HealthCheckStatus.LastCheckDate,
                                                        CheckDate = subscription.HealthCheckStatus.CheckDate,
                                                        Duration = subscription.HealthCheckStatus.Duration,
                                                        HealthyCount = subscription.HealthCheckStatus.HealthyCount,
                                                        UnhealthyCount = subscription.HealthCheckStatus.UnhealthyCount,
                                                        IsChecked = subscription.HealthCheckStatus.IsChecked,
                                                    },
                                                    Specifications = subscription.SpecificationsValues.Select(specVal => new SpecificationListItemDto
                                                    {
                                                        Id = specVal.Specification.Id,
                                                        DisplayName = specVal.Specification.DisplayName,
                                                        Description = specVal.Specification.Description,
                                                        Name = specVal.Specification.Name,
                                                        DataType = specVal.Specification.DataType,
                                                        InputType = specVal.Specification.InputType,
                                                        IsRequired = specVal.Specification.IsRequired,
                                                        IsUserEditable = specVal.Specification.IsUserEditable,
                                                        ValidationFailureDescription = specVal.Specification.ValidationFailureDescription,
                                                        RegularExpression = specVal.Specification.RegularExpression,
                                                        Value = specVal.Value,
                                                    })
                                                }),
                                            })
                                            .SingleOrDefaultAsync(cancellationToken);
            if (tenant is not null)
            {
                foreach (var subscription in tenant.Subscriptions)
                {
                    // Set Actions
                    var stages = await _workflow.GetNextStagesAsync(expectedResourceStatus: subscription.ExpectedResourceStatus, currentStatus: subscription.Status,
                                                                       currentStep: subscription.Step,
                                                                       userType: _identityContextService.GetUserType());
                    subscription.Actions = stages.ToActionsResults();
                    subscription.HealthCheckUrl = subscription.HealthCheckUrl.Replace("{name}", tenant.UniqueName);

                    // Set ShowHealthStatus
                    subscription.HealthCheckStatus.ShowHealthStatus = IsMustShowHealthStatus(subscription.HealthCheckStatus, subscription.Status);

                    // Try retrieve last External System Dispatch if existed
                    subscription.HealthCheckStatus.ExternalSystemDispatch = await TryGetExternalSystemDispatchesAsync(subscription.ProductId, tenant.Id, cancellationToken);
                }
            }

            return Result<TenantDto>.Successful(tenant);
        }

        private async Task<ExternalSystemDispatchDto?> TryGetExternalSystemDispatchesAsync(Guid productId, Guid tenantId, CancellationToken stoppingToken)
        {
            return await _dbContext.ExternalSystemDispatches
                                               .AsNoTracking()
                                               .Where(x => x.ProductId == productId && x.TenantId == tenantId)
                                               .OrderByDescending(x => x.TimeStamp)
                                               .Select(x => new ExternalSystemDispatchDto
                                               {
                                                   IsSuccessful = x.IsSuccessful,
                                                   DispatchDate = x.DispatchDate,
                                                   Url = x.Url,
                                                   Notes = x.Notes,
                                                   Duration = x.Duration,
                                               })
                                               .FirstOrDefaultAsync(stoppingToken);
        }

        private bool IsMustShowHealthStatus(ProductTenantHealthStatusDto healthCheckStatus, TenantStatus tenantStatus)
        {
            if (tenantStatus != TenantStatus.Active && tenantStatus != TenantStatus.CreatedAsActive)
            {
                return false;
            }

            healthCheckStatus.ShowHealthStatus = true;
            if (!healthCheckStatus.IsChecked)
            {
                healthCheckStatus.ShowHealthStatus = false;
            }

            return healthCheckStatus.ShowHealthStatus;
        }
        #endregion
    }
}
