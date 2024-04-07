using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.ApplyDowngradeToSubscription;

public partial class ApplyDowngradeToSubscriptionCommandValidator : AbstractValidator<ApplyDowngradeToSubscriptionCommand>
{
    public ApplyDowngradeToSubscriptionCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.TenantName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}
