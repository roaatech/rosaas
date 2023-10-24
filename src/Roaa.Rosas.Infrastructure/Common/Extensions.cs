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
            return context.ChangeTracker
                .Entries<IBaseEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .SelectMany(e =>
                {
                    var domainEvents = e.DomainEvents.ToList();
                    e.ClearDomainEvents();
                    return domainEvents;
                })
                .ToList();
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