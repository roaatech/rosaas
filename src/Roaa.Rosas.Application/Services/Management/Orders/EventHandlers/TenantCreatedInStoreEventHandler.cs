using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.Specifications;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.EventHandlers
{
    public class TenantCreatedInStoreEventHandler : IInternalDomainEventHandler<TenantCreatedInStoreEvent>
    {
        private readonly ILogger<TenantCreatedInStoreEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly ITenantWorkflow _workflow;
        private readonly IIdentityContextService _identityContextService;
        private readonly ITenantService _tenantService;
        private readonly IPaymentService _paymentService;
        private readonly ISpecificationService _specificationService;
        private readonly ISettingService _settingService;
        private readonly IEntityAdminPrivilegeService _tenantAdminService;

        public TenantCreatedInStoreEventHandler(ITenantWorkflow workflow,
                                              IRosasDbContext dbContext,
                                                IIdentityContextService identityContextService,
                                                ITenantService tenantService,
                                                IPaymentService paymentService,
                                                ISpecificationService specificationService,
                                                ISettingService settingService,
                                                IEntityAdminPrivilegeService tenantAdminService,
                                                ILogger<TenantCreatedInStoreEventHandler> logger)
        {
            _workflow = workflow;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
            _paymentService = paymentService;
            _specificationService = specificationService;
            _settingService = settingService;
            _tenantAdminService = tenantAdminService;
            _logger = logger;
        }

        public async Task Handle(TenantCreatedInStoreEvent @event, CancellationToken cancellationToken)
        {

            var orderItems = await _dbContext.OrderItems
                                        .Where(x => x.OrderId == @event.Tenant.LastOrderId)
                                        .ToListAsync(cancellationToken);

            foreach (var item in orderItems)
            {
                ICollection<Subscription> subscriptions = @event.Tenant.Subscriptions ?? new List<Subscription>();

                if (subscriptions.Any())
                {
                    subscriptions = await _dbContext.Subscriptions
                                      .Where(x => x.TenantId == @event.Tenant.Id)
                                      .ToListAsync(cancellationToken);
                }

                var subscription = subscriptions.Where(x => x.ProductId == item.ProductId &&
                                                            x.PlanId == item.PlanId &&
                                                            x.PlanPriceId == item.PlanPriceId)
                                                .FirstOrDefault();



                if (subscription is null)
                {
                    throw new NullReferenceException($"The subscription can't be null.");
                }

                item.SubscriptionId = subscription.Id;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }


    }
}
