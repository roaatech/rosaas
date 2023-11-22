using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Infrastructure.Common
{
    public static class Extensions
    {
        public static EntityTypeBuilder ToTableName(this EntityTypeBuilder builder, string? name)
        {
            return builder.ToTable(name.ToTableNamingStrategy());
        }

        public static string ToTableNamingStrategy(this string name)
        {
            return name.ToSnakeCaseNamingStrategy();
        }

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