﻿using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Subscription : BaseAuditableEntity
    {


        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }



        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
        public TenantStatus Status { get; set; }
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public string Metadata { get; set; } = string.Empty;
        public virtual Plan? Plan { get; set; }
        public virtual PlanPrice? PlanPrice { get; set; }
        public virtual Tenant? Tenant { get; set; }
        public virtual Product? Product { get; set; }
        public virtual TenantHealthStatus? HealthCheckStatus { get; set; }
        public virtual ICollection<SubscriptionFeature>? SubscriptionFeatures { get; set; }

    }

}