using FluentValidation;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients.Validators
{
    public class CreateClientByProductModelValidator : AbstractValidator<CreateClientByProductModel>
    {
        public CreateClientByProductModelValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
            RuleFor(x => x.ProductOwnerClientId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
            RuleFor(x => x.ClientId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);
            RuleFor(x => x.DisplayName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        }
    }

}
