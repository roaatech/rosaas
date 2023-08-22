﻿using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantById
{
    public record TenantDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UniqueName { get; set; } = string.Empty;
        public IEnumerable<ProductTenantDto> Products { get; set; } = new List<ProductTenantDto>();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }

    }

    public record ProductTenantDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public TenantStatus Status { get; set; }
        public DateTime EditedDate { get; set; }
        public object Metadata { get; set; } = new();
        public ProductTenantHealthStatusDto HealthCheckStatus { get; set; } = new();
        public IEnumerable<ActionResultModel> Actions { get; set; } = new List<ActionResultModel>();
    }

    public class ProductTenantHealthStatusDto
    {
        public bool ShowHealthStatus { get; set; } = true;

        public bool IsHealthy { get; set; }

        public string HealthCheckUrl { get; set; } = string.Empty;

        public int Duration { get; set; }

        public DateTime LastCheckDate { get; set; }

        public DateTime CheckDate { get; set; }

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

}