using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.TenantCreationRequests;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenant;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.EventHandlers
{
    public class OrderCompletionAchievedForTenantCreationEventHandler : IInternalDomainEventHandler<OrderCompletionAchievedForTenantCreationEvent>
    {
        private readonly ILogger<OrderCompletionAchievedForTenantCreationEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ITenantCreationRequestService _tenantCreationRequestService;
        private readonly ISender _mediator;

        public OrderCompletionAchievedForTenantCreationEventHandler(ITenantWorkflow workflow,
                                            IRosasDbContext dbContext,
                                            IIdentityContextService identityContextService,
                                            ISubscriptionService subscriptionService,
                                            ITenantCreationRequestService tenantCreationRequestService,
                                            ISender mediator,
                                            ILogger<OrderCompletionAchievedForTenantCreationEventHandler> logger)
        {
            _workflow = workflow;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _subscriptionService = subscriptionService;
            _tenantCreationRequestService = tenantCreationRequestService;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(OrderCompletionAchievedForTenantCreationEvent @event, CancellationToken cancellationToken)
        {
            var tenantCreationRequest = await _dbContext.TenantCreationRequests
                                                        .Include(x => x.Specifications)
                                                        .Where(x => x.OrderId == @event.OrderId)
                                                        .SingleOrDefaultAsync(cancellationToken);

            var order = await _dbContext.Orders.Where(x => x.Id == @event.OrderId)
                                             .Include(x => x.OrderItems)
                                             .SingleOrDefaultAsync(cancellationToken);

            var model = new TenantCreationRequestModel
            {
                DisplayName = tenantCreationRequest.DisplayName,
                SystemName = tenantCreationRequest.NormalizedSystemName,
                Subscriptions = order.OrderItems.Select(orderItem =>
                                                        new CreateSubscriptionModel
                                                        {
                                                            CustomPeriodInDays = orderItem.CustomPeriodInDays,
                                                            PlanId = orderItem.PlanId,
                                                            PlanPriceId = orderItem.PlanPriceId,
                                                            ProductId = orderItem.ProductId,
                                                            Specifications = tenantCreationRequest
                                                                                        .Specifications
                                                                                        .Where(x => x.ProductId == orderItem.ProductId)
                                                                                        .Select(spec =>
                                                                                                    new CreateSpecificationValueModel
                                                                                                    {
                                                                                                        SpecificationId = spec.SpecificationId,
                                                                                                        Value = spec.Value,
                                                                                                    })
                                                                                        .ToList(),
                                                        })
                                                        .ToList(),
            };

            var preparationsResult = await _tenantCreationRequestService.PrepareTenantCreationAsync(model, tenantCreationRequest.Id, cancellationToken);

            if (!preparationsResult.Success)
            {
                throw new Exception(String.Join(", ", preparationsResult.Messages.Select(x => x.Message)));
            }

            var tenantCreatedResult = await _mediator.Send(
                                                      new CreateTenantCommand
                                                      {
                                                          PlanDataList = preparationsResult.Data,
                                                          DisplayName = model.DisplayName,
                                                          SystemName = model.SystemName,
                                                          Subscriptions = model.Subscriptions,
                                                          OrderId = order.Id,
                                                          UserId = order.CreatedByUserId,
                                                          UserType = order.CreatedByUserType,
                                                      },
                                                      cancellationToken);
            if (!tenantCreatedResult.Success)
            {
                throw new Exception(String.Join(", ", tenantCreatedResult.Messages.Select(x => x.Message)));
            }


            var productsIds = tenantCreatedResult.Data.Products.Select(x => x.ProductId).ToList();

            var tenantSystemNames = await _dbContext.TenantSystemNames.Where(x => productsIds.Contains(x.ProductId) &&
                                                               model.SystemName.ToUpper().Equals(x.TenantNormalizedSystemName))
                                                   .ToListAsync(cancellationToken);
            foreach (var tenantSystemName in tenantSystemNames)
            {
                tenantSystemName.TenantId = tenantCreatedResult.Data.TenantId;
            }

            order.TenantId = tenantCreatedResult.Data.TenantId;

            await _dbContext.SaveChangesAsync(cancellationToken);

        }

    }
}
