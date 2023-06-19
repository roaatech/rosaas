using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Tenants.Commands.CreateTenant;

public record TenantCreatedResultDto
{
    public TenantCreatedResultDto(Guid id, TenantStatus status, IEnumerable<ActionResultModel> actions)
    {
        Id = id;
        Status = status;
        Actions = actions;
    }

    public Guid Id { get; set; }
    public TenantStatus Status { get; set; }
    public IEnumerable<ActionResultModel> Actions { get; set; } = new List<ActionResultModel>();
}

