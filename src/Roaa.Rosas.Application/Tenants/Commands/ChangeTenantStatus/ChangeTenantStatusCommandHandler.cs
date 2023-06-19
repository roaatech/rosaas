using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Tenants.Service;
using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;

public class ChangeTenantStatusCommandHandler : IRequestHandler<ChangeTenantStatusCommand, Result<TenantStatusChangedResultDto>>
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
    public async Task<Result<TenantStatusChangedResultDto>> Handle(ChangeTenantStatusCommand request, CancellationToken cancellationToken)
    {
        var result = await _tenantService.ChangeTenantStatusAsync(new ChangeTenantStatusModel
        {
            UserType = _identityContextService.GetUserType(),
            Status = request.Status,
            Action = Domain.Entities.Management.WorkflowAction.Ok,
            TenantId = request.TenantId,
            EditorBy = _identityContextService.GetActorId(),
        });


        if (!result.Success)
        {
            return Result<TenantStatusChangedResultDto>.Fail(result.Messages);
        }

        var statusManager = TenantStatusManager.FromKey(result.Data.Tenant.Status);

        await statusManager.PublishEventAsync(_publisher, result.Data.Tenant, result.Data.Process.CurrentStatus, cancellationToken);

        var updatedStatus = await _dbContext.Tenants.Where(x => x.Id == request.TenantId)
                                                    .Select(x => x.Status)
                                                    .SingleOrDefaultAsync();


        var flows = await _workflow.GetProcessActionsAsync(updatedStatus, _identityContextService.GetUserType());

        return Result<TenantStatusChangedResultDto>.Successful(new TenantStatusChangedResultDto(updatedStatus, flows.ToActionsResults()));
    }

    #endregion
}

