using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.Commands.HandleExpiredSubscription;
public record HandleExpiredSubscriptionCommand : IRequest<Result>
{
    public int RangeInHoursBetweenDates { get; set; } = 1;
    public HandleExpiredSubscriptionCommand(int rangeInHoursBetweenDates)
    {
        RangeInHoursBetweenDates = rangeInHoursBetweenDates;
    }
}