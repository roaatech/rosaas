using FluentValidation;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Tenants.Commands.UpdateTenant;

public partial class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.Id).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        // RuleFor(x => x.UniqueName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        //  RuleFor(x => x.UniqueName).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
    }
}
