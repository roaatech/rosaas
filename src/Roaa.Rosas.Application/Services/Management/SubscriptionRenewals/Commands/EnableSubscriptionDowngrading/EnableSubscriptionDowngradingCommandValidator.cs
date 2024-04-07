using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.EnableSubscriptionDowngrading;

public partial class EnableSubscriptionDowngradingCommandValidator : AbstractValidator<EnableSubscriptionDowngradingCommand>
{
    public EnableSubscriptionDowngradingCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.SubscriptionId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PlanId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PlanPriceId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.CardReferenceId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PaymentPlatform).IsInEnum().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
    }
}
