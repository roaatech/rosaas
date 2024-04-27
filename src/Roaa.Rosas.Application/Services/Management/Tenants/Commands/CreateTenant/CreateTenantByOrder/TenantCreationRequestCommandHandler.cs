using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.TenantCreationRequests;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenant;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantByOrder;
public class CreateTenantByOrderEventHandler : IRequestHandler<CreateTenantByOrderCommand, Result>
{
    private readonly ILogger<CreateTenantByOrderEventHandler> _logger;
    private readonly IRosasDbContext _dbContext;
    private readonly ITenantWorkflow _workflow;
    private readonly IIdentityContextService _identityContextService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ITenantCreationRequestService _tenantCreationRequestService;
    private readonly ISubscriptionRenewalService _subscriptionAutoRenewalService;
    private readonly ISender _mediator;

    public CreateTenantByOrderEventHandler(ITenantWorkflow workflow,
                                        IRosasDbContext dbContext,
                                        IIdentityContextService identityContextService,
                                        ISubscriptionService subscriptionService,
                                        ITenantCreationRequestService tenantCreationRequestService,
                                        ISubscriptionRenewalService subscriptionAutoRenewalService,
                                        ISender mediator,
                                        ILogger<CreateTenantByOrderEventHandler> logger)
    {
        _workflow = workflow;
        _dbContext = dbContext;
        _identityContextService = identityContextService;
        _subscriptionService = subscriptionService;
        _tenantCreationRequestService = tenantCreationRequestService;
        _subscriptionAutoRenewalService = subscriptionAutoRenewalService;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateTenantByOrderCommand command, CancellationToken cancellationToken)
    {
        var tenantRequest = await _dbContext.TenantCreationRequests
                                                    .Include(x => x.Specifications)
                                                    .Where(x => x.OrderId == command.OrderId)
                                                    .SingleOrDefaultAsync(cancellationToken);

        var order = await _dbContext.Orders.Where(x => x.Id == command.OrderId)
                                         .Include(x => x.OrderItems)
                                         .SingleOrDefaultAsync(cancellationToken);

        var model = new TenantCreationRequestModel
        {
            DisplayName = tenantRequest.DisplayName,
            SystemName = tenantRequest.NormalizedSystemName,
            Subscriptions = order.OrderItems.Select(orderItem =>
            {

                var createSubscriptionModel = new CreateSubscriptionModel
                {
                    CustomPeriodInDays = orderItem.CustomPeriodInDays,
                    PlanId = orderItem.PlanId,
                    PlanPriceId = orderItem.PlanPriceId,
                    ProductId = orderItem.ProductId,
                    Specifications = tenantRequest
                                                  .Specifications
                                                  .Where(x => x.ProductId == orderItem.ProductId)
                                                  .Select(spec =>
                                                              new CreateSpecificationValueModel
                                                              {
                                                                  SpecificationId = spec.SpecificationId,
                                                                  Value = spec.Value,
                                                              })
                                                  .ToList(),
                    UserEnabledTheTrial = orderItem.TrialPeriodInDays != 0,
                };
                createSubscriptionModel.SetSequenceNum(orderItem.SequenceNum);
                return createSubscriptionModel;
            })
                                                    .ToList(),
        };

        var preparationsResult = await _tenantCreationRequestService.PrepareTenantCreationAsync(model, tenantRequest.Id, cancellationToken);

        if (!preparationsResult.Success)
        {
            throw new Exception(String.Join(", ", preparationsResult.Messages.Select(x => x.Message)));
        }

        var tenantCreatedResult = await _mediator.Send(
                                                  new CreateTenantCommand
                                                  {
                                                      Subscriptions = preparationsResult.Data,
                                                      DisplayName = model.DisplayName,
                                                      SystemName = model.SystemName,
                                                      OrderId = order.Id,
                                                      TenantRequestId = tenantRequest.Id,
                                                      UserId = order.CreatedByUserId,
                                                      UserType = order.CreatedByUserType,
                                                  },
                                                  cancellationToken);
        if (!tenantCreatedResult.Success)
        {
            throw new Exception(String.Join(", ", tenantCreatedResult.Messages.Select(x => x.Message)));
        }

        var productsIds = tenantCreatedResult.Data.Products.Select(x => x.ProductId).ToList();

        order.TenantId = tenantCreatedResult.Data.TenantId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Enable Auto-Renewal
        var subscriptions = await _dbContext.Subscriptions
                                             .Where(x => x.TenantId == tenantCreatedResult.Data.TenantId)
                                             .Select(x => new { x.Id, x.SubscriptionMode, x.PlanPriceId })
                                             .ToListAsync(cancellationToken);

        foreach (var subscription in subscriptions)
        {
            if (tenantRequest.AutoRenewalIsEnabled)
            {
                await _subscriptionAutoRenewalService.EnableAutoRenewalAsync(subscription.Id,
                                                                             command.CardReferenceId,
                                                                             command.PaymentPlatform,
                                                                             planPriceId: null,
                                                                             comment: null,
                                                                             order.PayerUserId.Value,
                                                                             order.PayerUserType,
                                                                             renewalsCount: 0,
                                                                             isContinuousRenewal: true,
                                                                             cancellationToken);
            }
        }

        return Result.Successful();
    }

}
