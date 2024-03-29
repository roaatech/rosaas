﻿using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantDeletedEvent : BaseInternalEvent
    {
        public Tenant Tenant { get; set; }
        public TenantStatus OldStatus { get; set; }


        public TenantDeletedEvent(Tenant tenant, TenantStatus oldStatus)
        {
            Tenant = tenant;
            OldStatus = oldStatus;
        }
    }
}
