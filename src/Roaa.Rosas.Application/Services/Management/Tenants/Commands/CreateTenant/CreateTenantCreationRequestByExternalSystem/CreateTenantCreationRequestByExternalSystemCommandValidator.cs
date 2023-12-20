using FluentValidation;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequestByExternalSystem;

public partial class CreateTenantCreationRequestByExternalSystemCommandValidator : AbstractValidator<CreateTenantCreationRequestByExternalSystemCommand>
{
    public CreateTenantCreationRequestByExternalSystemCommandValidator(IIdentityContextService identityContextService)
    {

        RuleFor(x => x.TenantSystemName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.TenantSystemName).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);

        RuleFor(x => x.PlanPriceSystemName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Specifications)
                   .Must(specification => !specification
                               .GroupBy(x => x.SystemName)
                               .Any(g => g.Count() > 1)
                         )
                  .WithError(ErrorMessage.SpecificationsIdsDuplicated, identityContextService.Locale);
    }
}

