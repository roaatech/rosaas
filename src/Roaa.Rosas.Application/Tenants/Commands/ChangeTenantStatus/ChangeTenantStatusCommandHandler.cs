using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Tenants.Service;
using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using System.Data;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;

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

        // #1 - Change Status   
        var result = await _tenantService.ChangeTenantStatusAsync(new ChangeTenantStatusModel
        {
            UserType = _identityContextService.GetUserType(),
            Status = request.Status,
            Action = WorkflowAction.Ok,
            TenantId = request.TenantId,
            ProductId = request.ProductId,
            EditorBy = _identityContextService.GetActorId(),
        });

        if (!result.Success)
        {
            return Result<List<TenantStatusChangedResultDto>>.Fail(result.Messages);
        }


        // #2 - Publish Events by status (Call External Systems)
        foreach (var resultItem in result.Data)
        {
            var statusManager = TenantStatusManager.FromKey(resultItem.ProductTenant.Status);

            await statusManager.PublishEventAsync(_publisher, resultItem.ProductTenant, resultItem.Process.CurrentStatus, cancellationToken);
        }

        // #3 - Retrieve The Results (Updated Status & Process Actions)
        Expression<Func<ProductTenant, bool>> predicate = x => x.TenantId == request.TenantId;
        if (request.ProductId is not null)
        {
            predicate = x => x.TenantId == request.TenantId && x.ProductId == request.ProductId;
        }

        var updatedStatuses = await _dbContext.ProductTenants
                                            .Where(predicate)
                                            .Select(x => new { x.Status, x.ProductId })
                                            .ToListAsync(cancellationToken);

        List<TenantStatusChangedResultDto> results = new();
        foreach (var item in updatedStatuses)
        {
            results.Add(new TenantStatusChangedResultDto(
            item.ProductId,
            item.Status,
            (await _workflow.GetProcessActionsAsync(item.Status,
                                                    _identityContextService.GetUserType()))
                            .ToActionsResults()));
        }

        return Result<List<TenantStatusChangedResultDto>>.Successful(results);
    }

    #endregion
}

