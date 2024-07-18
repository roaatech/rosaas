using FluentValidation;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models.Password;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Identity.Accounts.Validators.Password;

public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
{
    public ForgotPasswordModelValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.Email).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Email).EmailAddress()
            .WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
    }
}