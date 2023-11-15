using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantById
{
    public record TenantDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UniqueName { get; set; } = string.Empty;
        public IEnumerable<SubscriptionDto> Subscriptions { get; set; } = new List<SubscriptionDto>();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }

    }

    public record SubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public TenantStatus Status { get; set; }
        public TenantStep Step { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public object Metadata { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LookupItemDto<Guid> Plan { get; set; } = new();
        public LookupItemDto<Guid> Product { get; set; } = new();
        public ProductTenantHealthStatusDto HealthCheckStatus { get; set; } = new();
        public IEnumerable<ActionResultModel> Actions { get; set; } = new List<ActionResultModel>();
        public ExpectedTenantResourceStatus ExpectedResourceStatus { get; set; }
        public IEnumerable<SpecificationListItemDto> Specifications { get; set; } = new List<SpecificationListItemDto>();
    }

    public class ProductTenantHealthStatusDto
    {
        public bool ShowHealthStatus { get; set; } = true;

        public bool IsChecked { get; set; } = true;

        public bool IsHealthy { get; set; }

        public string HealthCheckUrl { get; set; } = string.Empty;

        public int Duration { get; set; }

        public DateTime LastCheckDate { get; set; }

        public DateTime CheckDate { get; set; }

        public int HealthyCount { get; set; }

        public int UnhealthyCount { get; set; }

        public ExternalSystemDispatchDto? ExternalSystemDispatch { get; set; }
    }

    public class ExternalSystemDispatchDto
    {
        public bool IsSuccessful { get; set; }

        public string Url { get; set; } = string.Empty;

        public DateTime DispatchDate { get; set; }

        public int Duration { get; set; }

        public string Notes { get; set; } = string.Empty;
    }

    public record SpecificationListItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public LocalizedString DisplayName { get; set; } = new();

        public LocalizedString Description { get; set; } = new();

        public SpecificationInputType InputType { get; set; }

        public SpecificationDataType DataType { get; set; }

        public bool IsRequired { get; set; }

        public bool IsUserEditable { get; set; }

        public string? RegularExpression { get; set; }

        public LocalizedString? ValidationFailureDescription { get; set; }

        public string? Value { get; set; }
    }
}
