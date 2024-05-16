using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.WebhookEndpoints.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints
{
    public class WebhookEndpointService : IWebhookEndpointService
    {
        private readonly IRosasDbContext _dbContext;

        public WebhookEndpointService(IRosasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<WebhookEndpointsListItem>>> GetWebhookEndpointsByEntityIdAsync(Guid entityId, EntityType entityType, CancellationToken cancellationToken = default)
        {
            var webhookEndpoints = await _dbContext.WebhookEndpoints
                                                     .AsNoTracking()
                                                     .Include(w => w.EventsToListen)
                                                     .Where(w => w.EntityId == entityId &&
                                                                 w.EntityType == entityType)
                                                     .ToListAsync(cancellationToken);

            var webhookEndpointsDto = webhookEndpoints.Select(w => new WebhookEndpointsListItem
            {
                Id = w.Id,
                Url = w.Url,
                Description = w.Description,
                IsActive = w.IsActive,
                EntityId = w.EntityId,
                EntityType = w.EntityType,
                SigningSecret = w.SigningSecret,
                EventsToListen = w.EventsToListen.Select(e => e.Event).ToList()
            }).ToList();

            return Result<List<WebhookEndpointsListItem>>.Successful(webhookEndpointsDto);
        }

        public async Task<Result<WebhookEndpointDto>> GetWebhookEndpointByIdAsync(Guid webhookEndpointId, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default)
        {
            var webhookEndpoint = await _dbContext.WebhookEndpoints
                                                   .AsNoTracking()
                                                   .Include(w => w.EventsToListen)
                                                   .Where(w => w.Id == webhookEndpointId &&
                                                               w.EntityId == entityId &&
                                                               w.EntityType == entityType)
                                                   .SingleOrDefaultAsync(cancellationToken);

            if (webhookEndpoint == null)
            {
                return Result<WebhookEndpointDto>.Fail($"Webhook endpoint with ID {webhookEndpointId} not found.");
            }

            var webhookEndpointDto = new WebhookEndpointDto
            {
                Id = webhookEndpoint.Id,
                Url = webhookEndpoint.Url,
                Description = webhookEndpoint.Description,
                IsActive = webhookEndpoint.IsActive,
                EntityId = webhookEndpoint.EntityId,
                EntityType = webhookEndpoint.EntityType,
                SigningSecret = webhookEndpoint.SigningSecret,
                EventsToListen = webhookEndpoint.EventsToListen
                    .Select(e => new WebhookEventDto
                    {
                        Id = e.Id,
                        Event = e.Event,
                    }).ToList()
            };

            return Result<WebhookEndpointDto>.Successful(webhookEndpointDto);
        }


        public async Task<Result<CreatedResult<Guid>>> CreateWebhookEndpointAsync(CreateWebhookEndpointModel model, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default)
        {
            var webhookEndpoint = new WebhookEndpoint
            {
                Url = model.Url,
                SigningSecret = model.SigningSecret,
                Description = model.Description,
                EntityId = model.EntityId,
                EntityType = model.EntityType,
                IsActive = model.IsActive,
                EventsToListen = model.EventsToListen.Select(x => new WebhookEndpointEvent
                {
                    Id = Guid.NewGuid(),
                    Event = x,

                }).ToList(),

            };

            _dbContext.WebhookEndpoints.Add(webhookEndpoint);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(webhookEndpoint.Id));
        }

        public async Task<Result> ChangeWebhookEndpointStatusAsync(Guid webhookEndpointId, bool isActive, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default)
        {
            var webhookEndpoint = await _dbContext.WebhookEndpoints
                                                  .Include(w => w.EventsToListen)
                                                  .Where(w => w.Id == webhookEndpointId &&
                                                              w.EntityId == entityId &&
                                                              w.EntityType == entityType)
                                                  .SingleOrDefaultAsync(cancellationToken);

            if (webhookEndpoint == null)
            {
                return Result.Fail("Webhook endpoint not found.");
            }

            webhookEndpoint.IsActive = isActive;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public async Task<Result> UpdateWebhookEndpointAsync(Guid webhookEndpointId, UpdateWebhookEndpointModel model, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default)
        {
            var webhookEndpoint = await _dbContext.WebhookEndpoints
                                                  .Where(w => w.Id == webhookEndpointId &&
                                                              w.EntityId == entityId &&
                                                              w.EntityType == entityType)
                                                  .Include(w => w.EventsToListen)
                                                  .SingleOrDefaultAsync(cancellationToken);
            if (webhookEndpoint == null)
            {
                return Result.Fail("Webhook endpoint not found.");
            }
            webhookEndpoint.Url = model.Url;
            webhookEndpoint.SigningSecret = model.SigningSecret;
            webhookEndpoint.Description = model.Description;
            webhookEndpoint.IsActive = model.IsActive;



            var eventsToListen = await _dbContext.WebhookEndpointEvents.Where(x => x.WebhookEndpointId == webhookEndpointId).ToListAsync();
            if (eventsToListen != null && eventsToListen.Any())
            {
                _dbContext.WebhookEndpointEvents.RemoveRange(eventsToListen);
            }
            _dbContext.WebhookEndpointEvents.AddRange(model.EventsToListen.Select(x => new WebhookEndpointEvent
            {
                Id = Guid.NewGuid(),
                Event = x,
                WebhookEndpointId = webhookEndpointId
            }));
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Successful();
        }
        public async Task<Result> DeleteWebhookEndpointAsync(Guid webhookEndpointId, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default)
        {
            var webhookEndpoint = await _dbContext.WebhookEndpoints
                                                  .Include(w => w.EventsToListen)
                                                  .Where(w => w.Id == webhookEndpointId &&
                                                              w.EntityId == entityId &&
                                                              w.EntityType == entityType)
                                                  .SingleOrDefaultAsync(cancellationToken);

            if (webhookEndpoint == null)
            {
                return Result.Fail("Webhook endpoint not found.");
            }

            _dbContext.WebhookEndpoints.Remove(webhookEndpoint);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }

    }
}
