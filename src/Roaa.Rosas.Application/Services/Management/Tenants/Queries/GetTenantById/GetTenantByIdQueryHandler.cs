using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
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
        #endregion


        #region Corts
        public GetTenantByIdQueryHandler(
            IRosasDbContext dbContext,
        ITenantWorkflow workflow,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _workflow = workflow;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Handler   
        public async Task<Result<TenantDto>> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {
            var tenant = await _dbContext.Tenants.AsNoTracking()
                                                 .Where(x => x.Id == request.Id)
                                                 .Select(tenant => new TenantDto
                                                 {
                                                     Id = tenant.Id,
                                                     UniqueName = tenant.UniqueName,
                                                     Title = tenant.Title,
                                                     CreatedDate = tenant.CreationDate,
                                                     EditedDate = tenant.ModificationDate,
                                                     Subscriptions = tenant.Subscriptions.Select(subscription => new SubscriptionDto
                                                     {
                                                         Product = new Common.Models.LookupItemDto<Guid>
                                                         {
                                                             Id = subscription.Product.Id,
                                                             Name = subscription.Product.Name,
                                                         },
                                                         ProductId = subscription.ProductId,
                                                         SubscriptionId = subscription.Id,
                                                         ProductName = subscription.Product.Name,
                                                         Status = subscription.Status,
                                                         CreatedDate = subscription.CreationDate,
                                                         EditedDate = subscription.ModificationDate,
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
                                                         },
                                                         Specifications = subscription.SpecificationsValues.Select(specVal => new SpecificationListItemDto
                                                         {
                                                             Id = specVal.Specification.Id,
                                                             DisplayName = specVal.Specification.DisplayName,
                                                             Description = specVal.Specification.Description,
                                                             Name = specVal.Specification.Name,
                                                             DataType = specVal.Specification.DataType,
                                                             FieldType = specVal.Specification.FieldType,
                                                             IsRequired = specVal.Specification.IsRequired,
                                                             IsUserEditable = specVal.Specification.IsUserEditable,
                                                             ValidationFailureDescription = specVal.Specification.ValidationFailureDescription,
                                                             RegularExpression = specVal.Specification.RegularExpression,
                                                             Value = specVal.Data,
                                                         })
                                                     }),
                                                 })
                                                 .SingleOrDefaultAsync(cancellationToken);
            if (tenant is not null)
            {
                foreach (var subscription in tenant.Subscriptions)
                {
                    // Set Actions
                    var flows = await _workflow.GetProcessActionsAsync(subscription.Status, _identityContextService.GetUserType());
                    subscription.Actions = flows.ToActionsResults();
                    subscription.HealthCheckUrl = subscription.HealthCheckUrl.Replace("{name}", tenant.UniqueName);

                    // Set ShowHealthStatus
                    subscription.HealthCheckStatus.ShowHealthStatus = IsMustShowHealthStatus(subscription.HealthCheckStatus, subscription.Status, tenant.CreatedDate);

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

        private bool IsMustShowHealthStatus(ProductTenantHealthStatusDto healthCheckStatus, TenantStatus tenantStatus, DateTime tenantCreatedDate)
        {
            if (tenantStatus != TenantStatus.Active && tenantStatus != TenantStatus.CreatedAsActive)
            {
                return false;
            }

            healthCheckStatus.ShowHealthStatus = true;
            if (!healthCheckStatus.IsHealthy &&
            healthCheckStatus.CheckDate == tenantCreatedDate &&
                healthCheckStatus.LastCheckDate == tenantCreatedDate)
            {
                healthCheckStatus.ShowHealthStatus = false;
            }

            return healthCheckStatus.ShowHealthStatus;
        }
        #endregion
    }
}
