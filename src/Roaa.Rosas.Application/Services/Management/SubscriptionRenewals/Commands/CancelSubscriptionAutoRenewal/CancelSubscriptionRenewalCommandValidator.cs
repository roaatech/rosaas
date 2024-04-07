using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.CancelSubscriptionRenewal;

public partial class CancelSubscriptionRenewalCommandValidator : AbstractValidator<CancelSubscriptionRenewalCommand>
{
    public CancelSubscriptionRenewalCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.SubscriptionRenewalId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.SubscriptionId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}
