using FluentValidation;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Identity.Auth.Validators;

public class CreateClientAdminUserByEmailValidator : CreateUserByEmailValidator<CreateClientAdminUserByEmailModel>
{
    public CreateClientAdminUserByEmailValidator(IIdentityContextService identityContextService) : base(identityContextService)
    {
        RuleFor(x => x.ClientId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

    }
}
