using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using System.Data;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;

public class ChangeTenantStatusCommandHandler : IRequestHandler<ChangeTenantStatusCommand, Result<List<TenantStatusChangedResultDto>>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly ITenantWorkflow _workflow;
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly ITenantService _tenantService;
    #endregion

    #region Corts
    public ChangeTenantStatusCommandHandler(
        IPublisher publisher,
        ITenantWorkflow workflow,
        IRosasDbContext dbContext,
        ITenantService tenantService,
        IIdentityContextService identityContextService)
    {
        _publisher = publisher;
        _workflow = workflow;
        _dbContext = dbContext;
        _tenantService = tenantService;
        _identityContextService = identityContextService;
    }

    #endregion


    #region Handler   
    public async Task<Result<List<TenantStatusChangedResultDto>>> Handle(ChangeTenantStatusCommand request, CancellationToken cancellationToken)
    {

        var tenantId = await _dbContext.Subscriptions
                                           .Where(x => x.ProductId == request.ProductId &&
                                                         request.TenantName.ToLower().Equals(x.Tenant.UniqueName))
                                           .Select(x => x.TenantId)
                                           .SingleOrDefaultAsync(cancellationToken);

        if (tenantId == Guid.Empty)
        {
            return Result<List<TenantStatusChangedResultDto>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(request.TenantName));
        }


        return await _tenantService.ChangeTenantStatusAsync(new ChangeTenantStatusModel(tenantId, request.Status, request.ProductId), cancellationToken);

    }

    #endregion
}
