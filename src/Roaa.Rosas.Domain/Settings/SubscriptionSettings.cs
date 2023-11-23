namespace Roaa.Rosas.Domain.Settings
{
    public class SubscriptionSettings : ISettings
    {
        public int SubscriptionWorkerTimePeriod { get; set; } = 4;
        public int AllowedPeriodTimeBeforeDeactivatingSubscriptionforNonPayment { get; set; } = 24;
    }
}
