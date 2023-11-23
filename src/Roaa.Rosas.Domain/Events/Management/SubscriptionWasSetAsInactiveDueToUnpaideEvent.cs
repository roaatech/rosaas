﻿using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionWasSetAsInactiveDueToUnpaideEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public string systemComment { get; set; } = string.Empty;

        public SubscriptionWasSetAsInactiveDueToUnpaideEvent()
        {
        }

        public SubscriptionWasSetAsInactiveDueToUnpaideEvent(Subscription subscription, string systemComment)
        {
            Subscription = subscription;
            this.systemComment = systemComment;
        }
    }
}
