using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.EventHandlers
{
    public class UserCreatedAsClientAdminEventHandler : IInternalDomainEventHandler<UserCreatedAsClientAdminEvent>
    {
        private readonly ILogger<UserCreatedAsClientAdminEventHandler> _logger;
        private readonly IEntityAdminPrivilegeService _tenantAdminService;

        public UserCreatedAsClientAdminEventHandler(ILogger<UserCreatedAsClientAdminEventHandler> logger,
                                     IEntityAdminPrivilegeService tenantAdminService)
        {
            _tenantAdminService = tenantAdminService;
            _logger = logger;
        }

        public async Task Handle(UserCreatedAsClientAdminEvent @event, CancellationToken cancellationToken)
        {
            await _tenantAdminService.CreateEntityAdminPrivilegeAsync(new Models.CreateEntityAdminPrivilegeModel
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

