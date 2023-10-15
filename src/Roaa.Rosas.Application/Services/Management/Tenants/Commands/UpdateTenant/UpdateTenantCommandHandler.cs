using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Result>
{
    #region Props 
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly BackgroundServicesStore _backgroundServicesStore;
    #endregion

    #region Corts
    public UpdateTenantCommandHandler(
        IRosasDbContext dbContext,
        IIdentityContextService identityContextService,
        BackgroundServicesStore backgroundServicesStore)
    {
        _dbContext = dbContext;
        _identityContextService = identityContextService;
        _backgroundServicesStore = backgroundServicesStore;
    }

    #endregion


    #region Handler   
    public async Task<Result> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {

        #region Validation

        var tenant = await _dbContext.Tenants.Where(x => x.Id == request.Id).SingleOrDefaultAsync();
        if (tenant is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }

        #endregion
        Tenant tenantBeforeUpdate = tenant.DeepCopy();

        DateTime date = DateTime.UtcNow;

        var processData = new TenantDataUpdatedProcessedData
        {
            OldData = new TenantInfoProcessedData { Title = tenant.Title },
            UpdatedData = new TenantInfoProcessedData { Title = request.Title },
        };

        tenant.Title = request.Title;
        tenant.ModifiedByUserId = _identityContextService.UserId;
        tenant.ModificationDate = date;

        var subscriptions = await _dbContext.Subscriptions.Where(x => x.TenantId == tenant.Id).ToListAsync();

        tenant.AddDomainEvent(new TenantUpdatedEvent(tenantBeforeUpdate, tenant));
        tenant.AddDomainEvent(new TenantProcessingCompletedEvent<TenantDataUpdatedProcessedData>(TenantProcessType.DataUpdated, true, processData, out _, subscriptions));

        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var subscription in subscriptions)
        {
            _backgroundServicesStore.RemoveTenantProcess(subscription.TenantId, subscription.ProductId);
        }

        return Result.Successful();
    }
    #endregion
}

