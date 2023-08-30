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
                                                     CreatedDate = tenant.Created,
                                                     EditedDate = tenant.Edited,
                                                     Products = tenant.Products.Select(x => new ProductTenantDto
                                                     {
                                                         Id = x.ProductId,
                                                         Name = x.Product.Name,
                                                         Status = x.Status,
                                                         EditedDate = x.Edited,
                                                         Metadata = x.Metadata,
                                                         HealthCheckUrl = x.HealthCheckUrl,
                                                         HealthCheckUrlIsOverridden = x.HealthCheckUrlIsOverridden,
                                                         HealthCheckStatus = new ProductTenantHealthStatusDto
                                                         {
                                                             HealthCheckUrl = x.HealthCheckStatus.HealthCheckUrl,
                                                             IsHealthy = x.HealthCheckStatus.IsHealthy,
                                                             LastCheckDate = x.HealthCheckStatus.LastCheckDate,
                                                             CheckDate = x.HealthCheckStatus.CheckDate,
                                                             Duration = x.HealthCheckStatus.Duration,
                                                             HealthyCount = x.HealthCheckStatus.HealthyCount,
                                                             UnhealthyCount = x.HealthCheckStatus.UnhealthyCount,
                                                         }
                                                     }),
                                                 })
                                                 .SingleOrDefaultAsync(cancellationToken);
            if (tenant is not null)
            {
                foreach (var product in tenant.Products)
                {
                    // Set Actions
                    var flows = await _workflow.GetProcessActionsAsync(product.Status, _identityContextService.GetUserType());
                    product.Actions = flows.ToActionsResults();
                    product.HealthCheckUrl = product.HealthCheckUrl.Replace("{name}", tenant.UniqueName);

                    // Set ShowHealthStatus
                    product.HealthCheckStatus.ShowHealthStatus = IsMustShowHealthStatus(product.HealthCheckStatus, product.Status, tenant.CreatedDate);

                    // Try retrieve last External System Dispatch if existed
                    product.HealthCheckStatus.ExternalSystemDispatch = await TryGetExternalSystemDispatchesAsync(product.Id, tenant.Id, cancellationToken);
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
