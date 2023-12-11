using FluentValidation;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Identity.Auth.Validators;

public class CreateProductAdminUserByEmailValidator : CreateUserByEmailValidator<CreateProductAdminUserByEmailModel>
{
    public CreateProductAdminUserByEmailValidator(IIdentityContextService identityContextService) : base(identityContextService)
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

    }
}
