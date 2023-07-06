using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Tenants.Commands.UpdateTenantMetadata;

public class UpdateTenantMetadataCommandHandler : IRequestHandler<UpdateTenantMetadataCommand, Result>
{
    #region Props 
    private readonly IRosasDbContext _dbContext;
    private readonly IIdentityContextService _identityContextService;
    #endregion

    #region Corts
    public UpdateTenantMetadataCommandHandler(
        IRosasDbContext dbContext,
        IIdentityContextService identityContextService)
    {
        _dbContext = dbContext;
        _identityContextService = identityContextService;
    }

    #endregion


    #region Handler   
    public async Task<Result> Handle(UpdateTenantMetadataCommand request, CancellationToken cancellationToken)
    {

        #region Validation

        var tenant = await _dbContext.ProductTenants
                                     .Where(x => x.TenantId == request.TenantId &&
                                                 x.ProductId == _identityContextService.GetProductId())
                                     .SingleOrDefaultAsync(cancellationToken);
        if (tenant is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
        }
        #endregion

        tenant.Metadata = request.Metadata;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Successful();
    }
    #endregion
}

