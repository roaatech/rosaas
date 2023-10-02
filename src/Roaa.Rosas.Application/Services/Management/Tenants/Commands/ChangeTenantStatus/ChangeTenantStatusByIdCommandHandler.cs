using MediatR;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;

public class ChangeTenantStatusByIdCommandHandler : IRequestHandler<ChangeTenantStatusByIdCommand, Result<List<TenantStatusChangedResultDto>>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly ITenantWorkflow _workflow;
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly ITenantService _tenantService;
    #endregion

    #region Corts
    public ChangeTenantStatusByIdCommandHandler(
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
    public async Task<Result<List<TenantStatusChangedResultDto>>> Handle(ChangeTenantStatusByIdCommand request, CancellationToken cancellationToken)
    {

        return await _tenantService.ChangeTenantStatusAsync(new ChangeTenantStatusModel(request.TenantId, request.Status, request.ProductId, request.Notes), cancellationToken);
    }

    #endregion
}
