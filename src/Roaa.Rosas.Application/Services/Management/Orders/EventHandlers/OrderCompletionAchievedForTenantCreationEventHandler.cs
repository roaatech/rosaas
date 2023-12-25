using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantInDB;
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
        private readonly ITenantService _tenantService;
        private readonly ISender _mediator;

        public OrderCompletionAchievedForTenantCreationEventHandler(ITenantWorkflow workflow,
                                            IRosasDbContext dbContext,
                                            IIdentityContextService identityContextService,
                                            ITenantService tenantService,
                                            ISender mediator,
                                            ILogger<OrderCompletionAchievedForTenantCreationEventHandler> logger)
        {
            _workflow = workflow;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(OrderCompletionAchievedForTenantCreationEvent @event, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders.Where(x => x.Id == @event.OrderId)
                                             .Include(x => x.OrderItems)
                                             .SingleOrDefaultAsync(cancellationToken);

            var tenantCreationRequest = await _dbContext.TenantCreationRequests
                                                        .Include(x => x.Specifications)
                                                        .Where(x => x.OrderId == @event.OrderId)
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

            var preparationsResult = await _tenantService.PrepareTenantCreationAsync(model, tenantCreationRequest.Id, cancellationToken);

            var se = String.Join(", ", preparationsResult.Messages);
            var seas = string.Join(", ", preparationsResult.Messages);
            if (!preparationsResult.Success)
            {
                throw new Exception(String.Join(", ", preparationsResult.Messages));
            }

            var tenantCreatedResult = await _mediator.Send(
                                                      new CreateTenantInDBCommand
                                                      {
                                                          PlanDataList = preparationsResult.Data,
                                                          DisplayName = model.DisplayName,
                                                          SystemName = model.SystemName,
                                                          Subscriptions = model.Subscriptions,
                                                          UserId = order.CreatedByUserId,
                                                          UserType = order.CreatedByUserType,
                                                      },
                                                      cancellationToken);
            if (!tenantCreatedResult.Success)
            {
                throw new Exception(String.Join(", ", preparationsResult.Messages));
            }

            order.TenantId = tenantCreatedResult.Data.Id;

            await _dbContext.SaveChangesAsync(cancellationToken);

        }

    }
}
