using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.PrepareSubscriptionReset;

public partial class PrepareSubscriptionResetCommandValidator : AbstractValidator<PrepareSubscriptionResetCommand>
{
    public PrepareSubscriptionResetCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.TenantId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
    }
}
