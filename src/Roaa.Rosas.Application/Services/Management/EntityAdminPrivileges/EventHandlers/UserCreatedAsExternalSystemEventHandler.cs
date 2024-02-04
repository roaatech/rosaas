using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.EventHandlers
{
    public class UserCreatedAsExternalSystemEventHandler : IInternalDomainEventHandler<UserCreatedAsExternalSystemEvent>
    {
        private readonly ILogger<UserCreatedAsExternalSystemEventHandler> _logger;
        private readonly IEntityAdminPrivilegeService _tenantAdminService;

        public UserCreatedAsExternalSystemEventHandler(ILogger<UserCreatedAsExternalSystemEventHandler> logger,
                                     IEntityAdminPrivilegeService tenantAdminService)
        {
            _tenantAdminService = tenantAdminService;
            _logger = logger;
        }

        public async Task Handle(UserCreatedAsExternalSystemEvent @event, CancellationToken cancellationToken)
        {
            //await _tenantAdminService.CreateEntityAdminPrivilegeAsync(new Models.CreateEntityAdminPrivilegeModel
            //{
            //    EntityId = @event.ProductId,
            //    EntityType = EntityType.Product,
            //    UserId = @event.User.Id,
            //    UserType = @event.User.UserType,
            //    IsMajor = @event.IsMajor,
            //});
        }
    }
}

