using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionTrials.Commands.UpgradeTrialSubscriptionToStandard;
public record UpgradeTrialSubscriptionToStandardCommand : IRequest<Result>
{
    public int RangeInHoursBetweenDates { get; set; } = 1;
    public UpgradeTrialSubscriptionToStandardCommand(int rangeInHoursBetweenDates)
    {
        RangeInHoursBetweenDates = rangeInHoursBetweenDates;
    }
}