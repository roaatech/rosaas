using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;

public partial class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.ProductsIds).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.UniqueName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.UniqueName).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
    }
}
