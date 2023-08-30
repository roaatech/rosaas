using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using static Roaa.Rosas.Domain.Entities.Management.TenantProcessData;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenantMetadata;

public class UpdateTenantMetadataCommandHandler : IRequestHandler<UpdateTenantMetadataCommand, Result>
{
    #region Props 
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly BackgroundServicesStore _backgroundServicesStore;
    #endregion

    #region Corts
    public UpdateTenantMetadataCommandHandler(
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
    public async Task<Result> Handle(UpdateTenantMetadataCommand request, CancellationToken cancellationToken)
    {

        #region Validation

        DateTime date = DateTime.UtcNow;

        var tenant = await _dbContext.ProductTenants
                                     .Where(x => x.ProductId == request.ProductId &&
                                                         request.TenantName.ToLower().Equals(x.Tenant.UniqueName))
                                     .SingleOrDefaultAsync(cancellationToken);
        if (tenant is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }
        #endregion 

        var processData = new TenantMetadataUpdatedProcessData
        {
            OldData = tenant.Metadata,
            UpdatedData = request.Metadata,
        };

        tenant.Metadata = System.Text.Json.JsonSerializer.Serialize(request.Metadata);

        var processHistory = new TenantProcessHistory
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            ProductId = tenant.ProductId,
            Status = tenant.Status,
            OwnerId = _identityContextService.GetActorId(),
            OwnerType = _identityContextService.GetUserType(),
            ProcessDate = date,
            TimeStamp = date,
            ProcessType = TenantProcessType.MetadataUpdated,
            Enabled = true,
            Data = System.Text.Json.JsonSerializer.Serialize(processData),
        };

        _dbContext.TenantProcessHistory.Add(processHistory);

        await _dbContext.SaveChangesAsync(cancellationToken);
        _backgroundServicesStore.RemoveTenantProcess(tenant.TenantId, tenant.ProductId);

        return Result.Successful();
    }
    #endregion
}

