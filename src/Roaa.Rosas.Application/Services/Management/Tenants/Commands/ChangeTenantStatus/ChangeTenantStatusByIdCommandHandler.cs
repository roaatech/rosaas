using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

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

        var any = await _dbContext.Tenants
                                    .AsNoTracking()
                                    .Where(x => _identityContextService.IsSuperAdmin() ||
                                                _dbContext.EntityAdminPrivileges
                                                            .Any(a =>
                                                                a.UserId == _identityContextService.UserId &&
                                                                a.EntityId == x.Id &&
                                                                a.EntityType == EntityType.Tenant
                                                                )
                                            )
                                    .AnyAsync(cancellationToken);
        if (!any)
        {
            return Result<List<TenantStatusChangedResultDto>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }

        return await _tenantService.SetTenantNextStatusAsync(tenantId: request.TenantId,
                                                            status: request.Status,
                                                            productId: request.ProductId,
                                                            action: request.Action,
                                                            expectedResourceStatus: null,
                                                            comment: request.Comment,
                                                            receivedRequestBody: null,
                                                            cancellationToken: cancellationToken);
    }

    #endregion
}
