using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.RenwalSubscription;
public record RenwalSubscriptionCommand : IRequest<Result>
{
    public int RangeInHoursBetweenDates { get; set; } = 1;
    public RenwalSubscriptionCommand(int rangeInHoursBetweenDates)
    {
        RangeInHoursBetweenDates = rangeInHoursBetweenDates;
    }
}