namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;

public record SpecificationInfoModel
{
    public Guid SpecificationId { get; set; }
    public Guid ProductId { get; set; }
}