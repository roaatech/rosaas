﻿using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Services
{
    public interface ITenantHealthCheckService
    {
        Task AddTenantHealthCheckStatusAsync(Type backgroundServiceType, JobTask jobTask, double duration, string healthCheckUrl, bool isAvailable, CancellationToken cancellationToken);

        Task AddExternalSystemDispatchAsync(JobTask jobTask, double duration, string url, bool isSuccessful, CancellationToken cancellationToken);

        TenantHealthCheckHistory AddTenantAvailabilityToDbContext(JobTask jobTask, double duration, string healthCheckUrl, bool isAvailable);

        JobTask AddJobTaskToDbContext(JobTask jobTask, JobTaskType type);

        Task<Result<ExternalSystemResultModel<dynamic>>> CheckTenantHealthStatusAsync(JobTask jobTask, string tenantHhealthCheckUrl, CancellationToken cancellationToken);

        Task<string> GetTenantHhealthCheckUrlAsync(JobTask jobTask, CancellationToken cancellationToken);

        Task AddInaccessibleJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken);


        Task AddUnavailableJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken);

        Task AddInformerJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken);

        Task RemoveAvailableJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken);

        Task RemoveUnavailableJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken);

        Task RemoveInaccessibleJobTaskTasks(JobTask jobTask, CancellationToken cancellationToken);

        Task RemoveJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken);

        Task<Result<ExternalSystemResultModel<dynamic>>> InformExternalSystemTheTenantIsUnavailableAsync(JobTask jobTask, ProductApiModel productApi, CancellationToken cancellationToken);

        Task<string> GetHealthCheckStatusUrlOfExternalSystemAsync(JobTask jobTask, IProductService productService, CancellationToken cancellationToken);


    }
}
