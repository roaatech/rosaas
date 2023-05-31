using MediatR;
using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Application.Interfaces
{
    public interface IInternalDomainEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : BaseInternalEvent
    {

    }
}
