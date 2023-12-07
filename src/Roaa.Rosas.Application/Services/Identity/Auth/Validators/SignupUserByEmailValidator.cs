using FluentValidation;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Identity.Auth.Validators;

public class SignupUserByEmailValidator : AbstractValidator<SignUpUserByEmailModel>
{
    public SignupUserByEmailValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.Password).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Email).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Email).EmailAddress().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithError(ErrorMessage.PasswordAndConfirmMustEqual, identityContextService.Locale);
    }
}