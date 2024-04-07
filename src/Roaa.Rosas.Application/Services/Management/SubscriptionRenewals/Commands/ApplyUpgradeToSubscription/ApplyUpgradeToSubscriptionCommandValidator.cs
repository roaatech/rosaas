using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.ApplyUpgradeToSubscription;

public partial class ApplyUpgradeToSubscriptionCommandValidator : AbstractValidator<ApplyUpgradeToSubscriptionCommand>
{
    public ApplyUpgradeToSubscriptionCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.TenantName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}
