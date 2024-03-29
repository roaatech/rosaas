﻿using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record ProductDto
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Url { get; set; } = string.Empty;
        public LookupItemDto<Guid> Client { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public string? DefaultHealthCheckUrl { get; set; }
        public string? HealthStatusChangeUrl { get; set; }
        public string? CreationEndpoint { get; set; }
        public string? ActivationEndpoint { get; set; }
        public string? DeactivationEndpoint { get; set; }
        public string? DeletionEndpoint { get; set; }
        public string? ApiKey { get; set; }
        public string? SubscriptionResetUrl { get; set; }
        public string? SubscriptionUpgradeUrl { get; set; }
        public string? SubscriptionDowngradeUrl { get; set; }
        public int WarningsNum { get; set; }
        public bool IsPublished { get; set; }
        public ProductTrialType TrialType { get; set; }
        public int TrialPeriodInDays { get; set; }
        public Guid? TrialPlanId { get; set; }
        public Guid? TrialPlanPriceId { get; set; }

    }
}
