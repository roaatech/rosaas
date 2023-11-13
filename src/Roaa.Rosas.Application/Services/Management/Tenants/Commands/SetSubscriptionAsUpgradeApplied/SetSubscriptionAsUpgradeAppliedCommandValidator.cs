using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.SetSubscriptionAsUpgradeApplied;

public partial class SetSubscriptionAsUpgradeAppliedCommandValidator : AbstractValidator<SetSubscriptionAsUpgradeAppliedCommand>
{
    public SetSubscriptionAsUpgradeAppliedCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.TenantName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}
