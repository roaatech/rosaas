using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.EventHandlers
{
    public class UserCreatedAsProductAdminEventHandler : IInternalDomainEventHandler<UserCreatedAsProductAdminEvent>
    {
        private readonly ILogger<UserCreatedAsProductAdminEventHandler> _logger;
        private readonly IEntityAdminPrivilegeService _tenantAdminService;

        public UserCreatedAsProductAdminEventHandler(ILogger<UserCreatedAsProductAdminEventHandler> logger,
                                     IEntityAdminPrivilegeService tenantAdminService)
        {
            _tenantAdminService = tenantAdminService;
            _logger = logger;
        }

        public async Task Handle(UserCreatedAsProductAdminEvent @event, CancellationToken cancellationToken)
        {
            await _tenantAdminService.CreateEntityAdminPrivilegeAsync(new Models.CreateResourceAdminModel
            {
                EntityId = @event.ProductId,
                EntityType = EntityType.Product,
                UserId = @event.User.Id,
                UserType = @event.User.UserType,
                IsMajor = @event.IsMajor,
            });
        }
    }
}

