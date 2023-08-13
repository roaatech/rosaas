using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;

public class ChangeTenantStatusByIdCommandValidator : AbstractValidator<ChangeTenantStatusByIdCommand>
{
    public ChangeTenantStatusByIdCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.TenantId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Status).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
    }
}

