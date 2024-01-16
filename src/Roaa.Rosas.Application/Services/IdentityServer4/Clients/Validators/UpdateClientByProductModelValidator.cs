using FluentValidation;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients.Validators
{
    public class UpdateClientByProductModelValidator : AbstractValidator<UpdateClientByProductModel>
    {
        public UpdateClientByProductModelValidator(IIdentityContextService identityContextService)
        {

            RuleFor(x => x.DisplayName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
            RuleFor(x => x.DisplayName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
            RuleFor(x => x.AccessTokenLifetimeInHour).GreaterThan(0).WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

        }
    }

}
