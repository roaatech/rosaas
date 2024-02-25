using FluentValidation;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Identity.Accounts.Validators
{
    public class ChangeMyPasswordModelValidator : AbstractValidator<ChangeMyPasswordModel>
    {
        public ChangeMyPasswordModelValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.NewPassword).NotEmpty()
                .WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.CurrentPassword).NotEmpty()
                .WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.CurrentPassword).NotEqual(x => x.NewPassword)
                .WithError(ErrorMessage.NewPasswordAndCurrentMustNotEqual, identityContextService.Locale);
        }
    }
}
