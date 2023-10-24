using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

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

        var subscription = await _dbContext.Subscriptions
                                     .Where(x => x.ProductId == request.ProductId &&
                                                         request.TenantName.ToLower().Equals(x.Tenant.UniqueName))
                                     .SingleOrDefaultAsync(cancellationToken);
        if (subscription is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }
        #endregion

        var metadataBeforeUpdate = subscription.Metadata;

        subscription.Metadata = System.Text.Json.JsonSerializer.Serialize(request.Metadata);

        subscription.AddDomainEvent(new TenantProcessingCompletedEvent(
            TenantProcessType.MetadataUpdated,
            true,
            new TenantMetadataUpdatedProcessedData(request.Metadata, metadataBeforeUpdate).Serialize(),
            out _,
            subscription));

        await _dbContext.SaveChangesAsync(cancellationToken);

        _backgroundServicesStore.RemoveTenantProcess(subscription.TenantId, subscription.ProductId);

        return Result.Successful();
    }
    #endregion
}

