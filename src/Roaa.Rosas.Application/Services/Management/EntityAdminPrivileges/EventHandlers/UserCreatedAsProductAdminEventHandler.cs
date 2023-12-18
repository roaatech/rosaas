using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.EventHandlers
{
    public class UserCreatedAsProductAdminEventHandler : IInternalDomainEventHandler<UserCreatedAsProductAdminEvent>
    {
        private readonly ILogger<UserCreatedAsProductAdminEventHandler> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IEntityAdminPrivilegeService _tenantAdminService;

        public UserCreatedAsProductAdminEventHandler(ILogger<UserCreatedAsProductAdminEventHandler> logger,
                                                     IRosasDbContext dbContext,
                                                     IEntityAdminPrivilegeService tenantAdminService)
        {
            _tenantAdminService = tenantAdminService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(UserCreatedAsProductAdminEvent @event, CancellationToken cancellationToken)
        {
            var models = await _dbContext.Subscriptions
                                        .Where(x => x.ProductId == @event.ProductId)
                                        .Select(x => new CreateEntityAdminPrivilegeModel
                                        {
                                            EntityId = x.TenantId,
                                            EntityType = EntityType.Tenant,
                                            UserId = @event.User.Id,
                                            UserType = @event.User.UserType,
                                            IsMajor = @event.IsMajor,
                                        })
                                        .Distinct()
                                        .ToListAsync(cancellationToken);

            models.Add(new CreateEntityAdminPrivilegeModel
            {
                EntityId = @event.ProductId,
                EntityType = EntityType.Product,
                UserId = @event.User.Id,
                UserType = @event.User.UserType,
                IsMajor = @event.IsMajor,
            });

            await _tenantAdminService.CreateEntityAdminPrivilegesAsync(models);
        }
    }
}

