using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.PrepareSubscriptionUpgrade;

public partial class PrepareSubscriptionUpgradeCommandValidator : AbstractValidator<PrepareSubscriptionUpgradeCommand>
{
    public PrepareSubscriptionUpgradeCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.SubscriptionId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PlanId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PlanPriceId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}
