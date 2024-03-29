﻿using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class TenantProcessingCompletedEventHandler : IInternalDomainEventHandler<TenantProcessingCompletedEvent>
    {
        private readonly ILogger<TenantProcessingCompletedEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;

        public TenantProcessingCompletedEventHandler(IRosasDbContext dbContext,
                                                     IIdentityContextService identityContextService,
                                                     ILogger<TenantProcessingCompletedEventHandler> logger)
        {
            _identityContextService = identityContextService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(TenantProcessingCompletedEvent @event, CancellationToken cancellationToken)
        {
            DateTime date = DateTime.UtcNow;


            var notes = new List<ProcessNote>();
            if (!string.IsNullOrWhiteSpace(@event.Comment))
                notes.Add(new ProcessNote(Common.Enums.UserType.SuperAdmin, @event.Comment));
            if (!string.IsNullOrWhiteSpace(@event.SystemComment))
                notes.Add(new ProcessNote(Common.Enums.UserType.RosasSystem, @event.SystemComment));

            foreach (var subscription in @event.Subscriptions)
            {
                var processHistory = new TenantProcessHistory
                {
                    Id = @event.ProcessId,
                    TenantId = subscription.TenantId,
                    ProductId = subscription.ProductId,
                    SubscriptionId = subscription.Id,
                    Status = subscription.Status,
                    Step = subscription.Step,
                    ExpectedResourceStatus = subscription.ExpectedResourceStatus,
                    OwnerId = _identityContextService.GetActorId(),
                    OwnerType = _identityContextService.GetUserType(),
                    ProcessDate = date,
                    TimeStamp = date,
                    ProcessType = @event.ProcessType,
                    Enabled = @event.Enabled,
                    Data = @event.ProcessedData,
                    Notes = notes,
                };

                _dbContext.TenantProcessHistory.Add(processHistory);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
