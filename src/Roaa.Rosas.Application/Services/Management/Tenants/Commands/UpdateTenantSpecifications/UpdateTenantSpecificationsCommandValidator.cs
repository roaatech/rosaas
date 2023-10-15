using FluentValidation;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenantSpecifications;

public partial class UpdateTenantSpecificationsCommandValidator : AbstractValidator<UpdateTenantSpecificationsCommand>
{
    public UpdateTenantSpecificationsCommandValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.Id).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Specifications).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Specifications)
     .Must(specification => !specification
                 .GroupBy(x => x.SpecificationId)
                 .Any(g => g.Count() > 1)
           )
     .WithError(ErrorMessage.SpecificationsIdsDuplicated, identityContextService.Locale);
    }
}
