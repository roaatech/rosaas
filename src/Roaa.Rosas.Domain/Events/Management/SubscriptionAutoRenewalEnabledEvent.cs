﻿using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionAutoRenewalEnabledEvent : BaseInternalEvent
    {
        public SubscriptionAutoRenewalEnabledEvent(SubscriptionAutoRenewal subscriptionAutoRenewal, string? comment)
        {
            SubscriptionAutoRenewal = subscriptionAutoRenewal;
            Comment = comment;
        }

        public SubscriptionAutoRenewal SubscriptionAutoRenewal { get; set; } = new();
        public string? Comment { get; set; }
    }
}
