using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Handlers
{
    public class ProductCreatedEventHandler : IInternalDomainEventHandler<ProductCreatedEvent>
    {
        private readonly ILogger<ProductCreatedEventHandler> _logger;
        private readonly IClientService _clientService;
        private readonly IRosasDbContext _dbContext;

        public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger,
                                          IClientService clientService,
                                          IRosasDbContext dbContext)
        {
            _logger = logger;
            _clientService = clientService;
            _dbContext = dbContext;
        }

        public async Task Handle(ProductCreatedEvent @event, CancellationToken cancellationToken)
        {
            var clientName = await _dbContext.Clients
                                         .Where(x => x.Id == @event.Product.ClientId)
                                         .Select(x => x.Name)
                                         .SingleOrDefaultAsync(cancellationToken);

            await _clientService.CreateClientAsync(new IdentityServer4.Clients.Models.CreateClientByProductModel
            {
                ClientId = $"{clientName}-{@event.Product.Name}",
                DisplayName = @event.Product.DisplayName,
                ProductId = @event.Product.Id,
                ProductOwnerClientId = @event.Product.ClientId,
                Description = @event.Product.DisplayName,
            }, cancellationToken);
        }
    }
}

