using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.EventHandlers
{
    public class UserCreatedAsClientAdminEventHandler : IInternalDomainEventHandler<UserCreatedAsClientAdminEvent>
    {
        private readonly ILogger<UserCreatedAsClientAdminEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IEntityAdminPrivilegeService _tenantAdminService;

        public UserCreatedAsClientAdminEventHandler(ILogger<UserCreatedAsClientAdminEventHandler> logger,
                                                     IRosasDbContext dbContext,
                                                     IEntityAdminPrivilegeService tenantAdminService)
        {
            _tenantAdminService = tenantAdminService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(UserCreatedAsClientAdminEvent @event, CancellationToken cancellationToken)
        {
            var models = await _dbContext.Products
                                            .Where(x => x.ClientId == @event.ClientId)
                                            .Select(x => new CreateEntityAdminPrivilegeModel
                                            {
                                                EntityId = x.Id,
                                                EntityType = EntityType.Product,
                                                UserId = @event.User.Id,
                                                UserType = @event.User.UserType,
                                                IsMajor = @event.IsMajor,
                                            })
                                            .Distinct()
                                            .ToListAsync(cancellationToken);

            var productsIds = models.Select(x => x.EntityId).ToList();

            models.AddRange(await _dbContext.Subscriptions
                                          .Where(x => productsIds.Contains(x.ProductId))
                                          .Select(x => new CreateEntityAdminPrivilegeModel
                                          {
                                              EntityId = x.TenantId,
                                              EntityType = EntityType.Tenant,
                                              UserId = @event.User.Id,
                                              UserType = @event.User.UserType,
                                              IsMajor = @event.IsMajor,
                                          })
                                          .Distinct()
                                          .ToListAsync(cancellationToken));


            models.Add(new CreateEntityAdminPrivilegeModel
            {
                EntityId = @event.ClientId,
                EntityType = EntityType.Client,
                UserId = @event.User.Id,
                UserType = @event.User.UserType,
                IsMajor = @event.IsMajor,
            });

        }
    }
}

