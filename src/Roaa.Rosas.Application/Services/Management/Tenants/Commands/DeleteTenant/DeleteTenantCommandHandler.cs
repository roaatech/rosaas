using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.DeleteTenant;

public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, Result>
{
    #region Props 
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    private readonly ITenantService _tenantService;
    private readonly ITenantWorkflow _workflow;
    #endregion

    #region Corts
    public DeleteTenantCommandHandler(
        IRosasDbContext dbContext,
        ITenantService tenantService,
        ITenantWorkflow workflow,
        IIdentityContextService identityContextService)
    {
        _dbContext = dbContext;
        _workflow = workflow;
        _tenantService = tenantService;
        _identityContextService = identityContextService;
    }

    #endregion


    #region Handler   
    public async Task<Result> Handle(DeleteTenantCommand model, CancellationToken cancellationToken)
    {
        var productTenants = await _dbContext.Subscriptions.Where(x => x.Id == model.TenantId).ToListAsync(cancellationToken);


        if (productTenants is not null && productTenants.Any(x => x.Status != TenantStatus.Deleted))
        {
            return Result.Fail(CommonErrorKeys.OperationFaild, _identityContextService.Locale);
        }

        var tenant = await _dbContext.Tenants.Where(x => x.Id == model.TenantId).SingleOrDefaultAsync();
        if (tenant is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }

        _dbContext.Tenants.Remove(tenant);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Successful();
    }

    #endregion
}

