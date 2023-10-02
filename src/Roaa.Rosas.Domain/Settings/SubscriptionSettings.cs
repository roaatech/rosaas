namespace Roaa.Rosas.Domain.Settings
{
    public class SubscriptionSettings : ISettings
    {
        public int SubscriptionWorkerTimePeriod { get; set; } = 6;
        public int AllowedPeriodTimeBeforeDeactivatingSubscriptionforNonPayment { get; set; } = 48;
    }
}
