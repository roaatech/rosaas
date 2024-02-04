﻿using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;


namespace Roaa.Rosas.Application.Services.Management.Plans.EventHandlers
{
    public class ProductCreatedEventHandler : IInternalDomainEventHandler<ProductCreatedEvent>
    {
        private readonly ILogger<ProductCreatedEventHandler> _logger;
        private readonly IPlanService _planService;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;

        public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger,
                                          IPlanService planService,
                                          IRosasDbContext dbContext,
                                          IIdentityContextService identityContextService)
        {
            _logger = logger;
            _planService = planService;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        public async Task Handle(ProductCreatedEvent @event, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;
            var userId = _identityContextService.UserId;

            _dbContext.Plans.AddRange(
               new Plan
               {
                   Id = Guid.NewGuid(),
                   ProductId = @event.Product.Id,
                   SystemName = "unlimited-internal",
                   DisplayName = "Open & Unlimited Internal",
                   Description = "This internal plan has been automatically generated by the system to be open and unlimited for product owners.",
                   DisplayOrder = 98,
                   CreatedByUserId = userId,
                   ModifiedByUserId = userId,
                   CreationDate = date,
                   ModificationDate = date,
                   TenancyType = TenancyType.Unlimited,
                   IsLockedBySystem = true,
                   Prices = new List<PlanPrice>
                   {
                             new PlanPrice
                            {
                                Id = Guid.NewGuid(),
                                SystemName = $"{@event.Product.SystemName}-open-{PlanCycle.Unlimited}".ToLower(),
                                PlanCycle = PlanCycle.Unlimited,
                                Price = decimal.Zero,
                                Description = "This internal plan price has been automatically generated by the system to be open and unlimited for product owners.",
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                CreationDate = date,
                                ModificationDate = date,
                                IsLockedBySystem = true,
                            }
                   }
               },
               new Plan
               {
                   Id = Guid.NewGuid(),
                   ProductId = @event.Product.Id,
                   SystemName = "limited-internal",
                   DisplayName = "Limited Internal",
                   Description = "This internal plan has been automatically generated by the system to be limited  and temporary for demonstrations by the Product Owner.",
                   DisplayOrder = 99,
                   CreatedByUserId = userId,
                   ModifiedByUserId = userId,
                   CreationDate = date,
                   ModificationDate = date,
                   TenancyType = TenancyType.Limited,
                   IsLockedBySystem = true,
                   Prices = new List<PlanPrice>
                   {
                             new PlanPrice
                            {
                                Id = Guid.NewGuid(),
                                SystemName = $"{@event.Product.SystemName}-{TenancyType.Limited}-{PlanCycle.Custom}".ToLower(),
                                PlanCycle = PlanCycle.Custom,
                                Price = decimal.Zero,
                                Description = "This internal plan price has been automatically generated by the system to be limited  and temporary for demonstrations by the Product Owner.",
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                CreationDate = date,
                                ModificationDate = date,
                                IsLockedBySystem = true,
                            }
                   }
               });

            await _dbContext.SaveChangesAsync(cancellationToken);


        }
    }
}

