using FluentValidation;
using Roaa.Rosas.Application.Services.IdentityServer4.Auth.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Auth.Validators
{
    public class AuthClientValidator : AbstractValidator<AuthClientModel>
    {
        public AuthClientValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.ClientId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            RuleFor(x => x.ClientSecret).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
        }
    }

}
