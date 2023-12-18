using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.IdentityServer4.EventHandlers
{
    public class IdentityServerClientCreatedEventHandler : IInternalDomainEventHandler<IdentityServerClientCreatedEvent>
    {
        private readonly ILogger<IdentityServerClientCreatedEventHandler> _logger;
        private readonly IAuthService _authService;

        public IdentityServerClientCreatedEventHandler(ILogger<IdentityServerClientCreatedEventHandler> logger,
                                                       IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public async Task Handle(IdentityServerClientCreatedEvent @event, CancellationToken cancellationToken)
        {
            await _authService.CreateExternalSystemUserByUsernameAsync(username: @event.Client.ClientId,
                                                                       productId: @event.CustomDetail.ProductId,
                                                                       userId: @event.CustomDetail.UserId,
                                                                       isLocked: true,
                                                                       cancellationToken);
        }
    }
}

