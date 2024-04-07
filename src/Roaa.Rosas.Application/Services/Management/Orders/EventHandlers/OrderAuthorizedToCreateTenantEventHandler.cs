using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.TenantCreationRequests;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.EventHandlers
{
    public class OrderAuthorizedToCreateTenantEventHandler : OrderCompletedToCreateTenantBaseEventHandler<OrderAuthorizedToCreateTenantEventHandler,
                                                                                                          OrderAuthorizedToCreateTenantEvent>,
                                                             IInternalDomainEventHandler<OrderAuthorizedToCreateTenantEvent>
    {

        public OrderAuthorizedToCreateTenantEventHandler(ITenantWorkflow workflow,
                                            IRosasDbContext dbContext,
                                            IIdentityContextService identityContextService,
                                            ISubscriptionService subscriptionService,
                                            ITenantCreationRequestService tenantCreationRequestService,
                                            ISubscriptionRenewalService subscriptionAutoRenewalService,
                                            ISender mediator,
                                            ILogger<OrderAuthorizedToCreateTenantEventHandler> logger) : base(workflow,
                                                                                                    dbContext,
                                                                                                    identityContextService,
                                                                                                    subscriptionService,
                                                                                                    tenantCreationRequestService,
                                                                                                    subscriptionAutoRenewalService,
                                                                                                    mediator,
                                                                                                    logger)
        {

        }

        public async Task Handle(OrderAuthorizedToCreateTenantEvent @event, CancellationToken cancellationToken)
        {
            await base.Handle(@event, cancellationToken);
        }

    }
}
