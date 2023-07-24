using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantById
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
        //public ProductTenantDto(Guid id, string? name, TenantStatus status, DateTime editedDate, Dictionary<string, string> metadata)
        //{
        //    Id = id;
        //    Name = name;
        //    Status = status;
        //    EditedDate = editedDate;
        //    Metadata = metadata ?? new();
        //}

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public TenantStatus Status { get; set; }
        public DateTime EditedDate { get; set; }
        public object Metadata { get; set; } = new();
        public IEnumerable<ActionResultModel> Actions { get; set; } = new List<ActionResultModel>();
    }
}
