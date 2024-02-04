using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.EventHandlers
{
    public class UserCreatedAsTenantAdminEventHandler : IInternalDomainEventHandler<UserCreatedAsTenantAdminEvent>
    {
        private readonly ILogger<UserCreatedAsTenantAdminEventHandler> _logger;
        private readonly IEntityAdminPrivilegeService _tenantAdminService;

        public UserCreatedAsTenantAdminEventHandler(ILogger<UserCreatedAsTenantAdminEventHandler> logger,
                                     IEntityAdminPrivilegeService tenantAdminService)
        {
            _tenantAdminService = tenantAdminService;
            _logger = logger;
        }

        public async Task Handle(UserCreatedAsTenantAdminEvent @event, CancellationToken cancellationToken)
        {
            await _tenantAdminService.CreateEntityAdminPrivilegeAsync(new Models.CreateEntityAdminPrivilegeModel
            {
                EntityId = @event.TenantId,
                EntityType = EntityType.Tenant,
                UserId = @event.User.Id,
                UserType = @event.User.UserType,
                IsMajor = @event.IsMajor,
            });
        }
    }
}

