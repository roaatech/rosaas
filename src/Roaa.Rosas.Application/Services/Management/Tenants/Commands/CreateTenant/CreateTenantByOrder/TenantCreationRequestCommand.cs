using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantByOrder;

public record CreateTenantByOrderCommand : IRequest<Result>
{
    public Guid OrderId { get; set; }

    public string CardReferenceId { get; set; }
    public PaymentPlatform PaymentPlatform { get; set; }

    public CreateTenantByOrderCommand(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform)
    {
        OrderId = orderId;
        CardReferenceId = cardReferenceId;
        PaymentPlatform = paymentPlatform;
    }
    public CreateTenantByOrderCommand()
    {
    }
}
