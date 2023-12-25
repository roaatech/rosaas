using FluentValidation;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.SystemMessages;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;

public partial class TenantCreationRequestCommandValidator : AbstractValidator<TenantCreationRequestCommand>
{
    public TenantCreationRequestCommandValidator(IIdentityContextService identityContextService)
    {

        //  RuleFor(x => x.SystemName).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        When(x => !string.IsNullOrWhiteSpace(x.SystemName), () =>
        {
            RuleFor(x => x.SystemName).Matches(@"^[a-zA-Z0-9?><;,{}[\]\-_]*$").WithError(CommonErrorKeys.InvalidParameters, identityContextService.Locale);
        });

        RuleForEach(x => x.Subscriptions).SetValidator(new CreateSubscriptionValidator(identityContextService));
    }
}

public partial class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionModel>
{
    public CreateSubscriptionValidator(IIdentityContextService identityContextService)
    {
        RuleFor(x => x.PlanPriceId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.ProductId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.PlanId).NotEmpty().WithError(CommonErrorKeys.ParameterIsRequired, identityContextService.Locale);

        RuleFor(x => x.Specifications)
         .Must(specification => !specification
                     .GroupBy(x => x.SpecificationId)
                     .Any(g => g.Count() > 1)
               )
         .WithError(ErrorMessage.SpecificationsIdsDuplicated, identityContextService.Locale);

    }
}
