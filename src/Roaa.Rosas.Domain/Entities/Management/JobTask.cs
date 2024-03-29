﻿namespace Roaa.Rosas.Domain.Entities.Management
{
    public class JobTask : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
        public Guid SubscriptionId { get; set; }
        public JobTaskType Type { get; set; }
        public DateTime CreationDate { get; set; }

    }

    public enum JobTaskType
    {
        Available = 1,
        Inaccessible = 2,
        Unavailable = 3,
        Informer = 4,
    }

}
