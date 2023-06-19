﻿using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantActivatedEvent : BaseInternalEvent
    {
        public Tenant Tenant { get; set; }
        public TenantStatus OldStatus { get; set; }

        public TenantActivatedEvent(Tenant tenant, TenantStatus oldStatus)
        {
            Tenant = tenant;
            OldStatus = oldStatus;
        }
    }
}
