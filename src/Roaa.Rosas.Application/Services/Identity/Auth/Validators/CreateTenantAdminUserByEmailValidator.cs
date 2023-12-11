using FluentValidation;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Identity.Auth.Validators;

public class CreateTenantAdminUserByEmailValidator : CreateUserByEmailValidator<CreateTenantAdminUserByEmailModel>
{
    public CreateTenantAdminUserByEmailValidator(IIdentityContextService identityContextService) : base(identityContextService)
    {
        RuleFor(x => x.TenantId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

    }
}