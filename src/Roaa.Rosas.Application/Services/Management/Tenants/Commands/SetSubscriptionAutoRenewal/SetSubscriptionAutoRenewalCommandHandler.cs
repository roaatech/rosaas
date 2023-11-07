using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.SetSubscriptionAutoRenewal;

public class SetSubscriptionAutoRenewalCommandHandler : IRequestHandler<SetSubscriptionAutoRenewalCommand, Result>
{
    #region Props 
    private readonly ILogger<SetSubscriptionAutoRenewalCommandHandler> _logger;
    private readonly IIdentityContextService _identityContextService;
    private readonly IRosasDbContext _dbContext;
    #endregion



    #region Corts
    public SetSubscriptionAutoRenewalCommandHandler(IIdentityContextService identityContextService,
                                                    IRosasDbContext dbContext,
                                                    ILogger<SetSubscriptionAutoRenewalCommandHandler> logger)
    {
        _identityContextService = identityContextService;
        _dbContext = dbContext;
        _logger = logger;
    }
    #endregion


    #region Handler   
    public async Task<Result> Handle(SetSubscriptionAutoRenewalCommand command, CancellationToken cancellationToken)
    {
        var planPrice = await _dbContext.PlanPrices
                                               .Where(x => x.Id == command.PlanPriceId)
                                               .SingleOrDefaultAsync();
        if (planPrice is null)
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(command.PlanPriceId));
        }

        if (!await _dbContext.Subscriptions
                                .Where(x => x.Id == command.SubscriptionId)
                                .AnyAsync())
        {
            return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(command.SubscriptionId));
        }

        var date = DateTime.UtcNow;
        var autoRenewal = await _dbContext.SubscriptionAutoRenewals
                              .Where(x => x.Id == command.SubscriptionId)
                                .SingleOrDefaultAsync();
        if (autoRenewal is null)
        {
            autoRenewal = new SubscriptionAutoRenewal
            {
                Id = command.SubscriptionId,
                SubscriptionId = command.SubscriptionId,
                PlanPriceId = command.PlanPriceId,
                PlanId = planPrice.PlanId,
                Cycle = planPrice.Cycle,
                Price = planPrice.Price,
                UpcomingAutoRenewalsCount = 1,
                IsPaid = true,
                Comment = command.Comment,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.SubscriptionAutoRenewals.Add(autoRenewal);
        }
        else
        {
            autoRenewal.PlanPriceId = command.PlanPriceId;
            autoRenewal.PlanId = planPrice.PlanId;
            autoRenewal.Cycle = planPrice.Cycle;
            autoRenewal.Price = planPrice.Price;
            autoRenewal.Comment = command.Comment;
            autoRenewal.ModifiedByUserId = _identityContextService.UserId;
            autoRenewal.ModificationDate = date;
        }




        autoRenewal.AddDomainEvent(new SubscriptionAutoRenewalEnabledEvent(autoRenewal, command.Comment));



        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Successful();
    }
    #endregion
}

