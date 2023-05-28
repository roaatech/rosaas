using MediatR;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Infrastructure.Common
{
    public static class Extensions
    {
        public static async Task<List<BaseInternalEvent>> FetchDomainEvents(this IMediator mediator, DbContext context)
        {
            var entities = context.ChangeTracker
                .Entries<IBaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity);

            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entities.ToList().ForEach(e => e.ClearDomainEvents());
            return domainEvents;

        }

        public static async Task DispatchDomainEvents(this IMediator mediator, List<BaseInternalEvent> domainEvents)
        {
            List<BaseInternalEvent> events = domainEvents.ToList();

            foreach (var domainEvent in events)
            {
                domainEvents.Remove(domainEvent);
                await mediator.Publish(domainEvent);
            }

            events.Clear();
        }
    }
}
