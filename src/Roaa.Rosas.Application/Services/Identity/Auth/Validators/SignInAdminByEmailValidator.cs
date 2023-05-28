using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;

namespace Roaa.Rosas.Application.Services.Identity.Auth.Validators
{
    public class SignInAdminByEmailValidator : AbstractValidator<SignInAdminByEmailModel>
    {
        public SignInAdminByEmailValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.Password).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.Email).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.Email).EmailAddress().WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
        }
    }

}
