using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.DeleteTenant;

public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.TenantId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}

