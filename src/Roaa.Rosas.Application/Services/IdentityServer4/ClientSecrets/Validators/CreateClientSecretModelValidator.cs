using FluentValidation;
using Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients.Validators
{
    public class CreateClientSecretModelValidator : AbstractValidator<CreateClientSecretModel>
    {
        public CreateClientSecretModelValidator(IIdentityContextService identityContextService)
        {
            RuleFor(x => x.Description).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

            When(model => model.Expiration is not null, () =>
            {
                RuleFor(x => x.Expiration).GreaterThan(DateTime.UtcNow).WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
            });
        }
    }

}
