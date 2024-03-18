using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models.TenantProcessHistoryData;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.EventHandlers
{
    public class SubscriptionFeaturesLimitsResetEventHandler : IInternalDomainEventHandler<SubscriptionFeaturesLimitsResetEvent>
    {
        private readonly ILogger<SubscriptionFeaturesLimitsResetEventHandler> _logger;
        private readonly IPublisher _publisher;
        private readonly IRosasDbContext _dbContext;

        public SubscriptionFeaturesLimitsResetEventHandler(
                                                 IPublisher publisher,
                                                 IRosasDbContext dbContext,
                                                 ILogger<SubscriptionFeaturesLimitsResetEventHandler> logger)
        {
            _publisher = publisher;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(SubscriptionFeaturesLimitsResetEvent @event, CancellationToken cancellationToken)
        {

            var features = @event.SubscriptionFeatures.Select(x => x.SystemName);

            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                processType: TenantProcessType.SubscriptionResetAppliedDone,
                                                enabled: true,
                                                processedData: new ProcessedDataOfSubscriptionFeatureLimitResetModel(features).Serialize(),
                                                comment: @event.Comment ?? string.Empty,
                                                systemComment: @event.SystemComment ?? string.Empty,
                                                processId: out _,
                                                @event.Subscription));
        }
    }
}
