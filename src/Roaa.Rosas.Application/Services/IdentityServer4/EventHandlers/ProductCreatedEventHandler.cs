using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.IdentityServer4.EventHandlers
{
    public class ProductCreatedEventHandler : IInternalDomainEventHandler<ProductCreatedEvent>
    {
        private readonly ILogger<ProductCreatedEventHandler> _logger;
        private readonly IClientService _clientService;
        private readonly IAuthService _authService;
        private readonly IRosasDbContext _dbContext;

        public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger,
                                          IClientService clientService,
                                          IAuthService authService,
                                          IRosasDbContext dbContext)
        {
            _logger = logger;
            _clientService = clientService;
            _authService = authService;
            _dbContext = dbContext;
        }

        public async Task Handle(ProductCreatedEvent @event, CancellationToken cancellationToken)
        {
            var clientName = await _dbContext.Clients
                                         .Where(x => x.Id == @event.Product.ClientId)
                                         .Select(x => x.Name)
                                         .SingleOrDefaultAsync(cancellationToken);

            string sysName = $"{clientName}-{@event.Product.Name}".ToLower();

            await _clientService.CreateClientAsync(new Clients.Models.CreateClientByProductModel
            {
                ClientId = sysName,
                DisplayName = @event.Product.DisplayName,
                ProductId = @event.Product.Id,
                ProductOwnerClientId = @event.Product.ClientId,
                Description = @event.Product.DisplayName,
            }, cancellationToken);

            await _authService.CreateExternalSystemUserByUsernameAsync(username: sysName,
                                                                       productId: @event.Product.Id,
                                                                       isLocked: true,
                                                                       cancellationToken);
        }
    }
}

