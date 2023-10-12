using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class TenantProcessingCompletedEventHandler<T> : IInternalDomainEventHandler<TenantProcessingCompletedEvent<T>> where T : TenantProcessedData
    {
        private readonly ILogger<TenantProcessingCompletedEventHandler<T>> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly BackgroundServicesStore _backgroundServicesStore;

        public TenantProcessingCompletedEventHandler(IRosasDbContext dbContext,
                                                     IIdentityContextService identityContextService,
                                                     BackgroundServicesStore backgroundServicesStore,
                                                     ILogger<TenantProcessingCompletedEventHandler<T>> logger)
        {
            _identityContextService = identityContextService;
            _backgroundServicesStore = backgroundServicesStore;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(TenantProcessingCompletedEvent<T> @event, CancellationToken cancellationToken)
        {
            DateTime date = DateTime.UtcNow;

            foreach (var subscription in @event.Subscriptions)
            {
                var processHistory = new TenantProcessHistory
                {
                    Id = @event.ProcessId,
                    TenantId = subscription.TenantId,
                    ProductId = subscription.ProductId,
                    SubscriptionId = subscription.Id,
                    Status = subscription.Status,
                    OwnerId = _identityContextService.GetActorId(),
                    OwnerType = _identityContextService.GetUserType(),
                    ProcessDate = date,
                    TimeStamp = date,
                    ProcessType = @event.ProcessType,
                    Enabled = @event.Enabled,
                    Data = @event.ProcessedData is null ? string.Empty : System.Text.Json.JsonSerializer.Serialize(@event.ProcessedData),
                    Notes = @event.Notes,
                };

                _dbContext.TenantProcessHistory.Add(processHistory);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
