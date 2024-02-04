using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CancelSubscriptionAutoRenewal;

public class CancelSubscriptionAutoRenewalCommandHandler : IRequestHandler<CancelSubscriptionAutoRenewalCommand, Result>
{
    #region Props 
    private readonly ILogger<CancelSubscriptionAutoRenewalCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public CancelSubscriptionAutoRenewalCommandHandler(IIdentityContextService identityContextService,
                                                    IRosasDbContext dbContext,
                                                    ILogger<CancelSubscriptionAutoRenewalCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(CancelSubscriptionAutoRenewalCommand command, CancellationToken cancellationToken)
    {
        var autoRenewal = await _dbContext.SubscriptionAutoRenewals
                                .Where(x => _identityContextService.IsSuperAdmin() ||
                                            _dbContext.EntityAdminPrivileges
                                                        .Any(a =>
                                                            a.UserId == _identityContextService.UserId &&
                                                            a.EntityId == x.Subscription.TenantId &&
                                                            a.EntityType == EntityType.Tenant
                                                            )
                                        )
                                .Where(x => x.Id == command.SubscriptionId)
                                  .SingleOrDefaultAsync();
        if (autoRenewal is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(command.SubscriptionId));
        }


        autoRenewal.AddDomainEvent(new SubscriptionAutoRenewalCanceledEvent(autoRenewal, command.Comment));

        _dbContext.SubscriptionAutoRenewals.Remove(autoRenewal);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Successful();
    }
    #endregion
}

