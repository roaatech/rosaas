using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionDowngrade;

public partial class RequestSubscriptionDowngradeCommandValidator : AbstractValidator<RequestSubscriptionDowngradeCommand>
{
    public RequestSubscriptionDowngradeCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.SubscriptionId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PlanId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PlanPriceId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.CardReferenceId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PaymentPlatform).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
    }
}
