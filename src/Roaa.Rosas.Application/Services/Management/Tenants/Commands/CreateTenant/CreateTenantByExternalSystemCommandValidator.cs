using FluentValidation;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;

public partial class CreateTenantByExternalSystemCommandValidator : AbstractValidator<CreateTenantByExternalSystemCommand>
{
    public CreateTenantByExternalSystemCommandValidator(IIdentityContextService identityContextService)
    {

        RuleFor(x => x.TenantName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.TenantName).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

        RuleFor(x => x.PlanName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PlanPriceName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Specifications)
                   .Must(specification => !specification
                               .GroupBy(x => x.Name)
                               .Any(g => g.Count() > 1)
                         )
                  .WithError(ErrorMessage.SpecificationsIdsDuplicated, identityContextService.Locale);
    }
}

