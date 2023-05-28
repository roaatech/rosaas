using MediatR;
using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Application.Interfaces
{
    public interface IInternalEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : BaseInternalEvent
    {

    }
}
